import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { getApiContainer } from '@/api/register';
import type { IGameRealtime, GameEvent, GameEventHandler } from '@/api/IGameRealtime';
import type {
  BunkerCardDto,
  ChatMessageDto,
  GamePhase,
  GameSnapshot,
  ParticipantDto,
  TallyDto,
} from '@/types/game.types';

export const useGameStore = defineStore('game', () => {
  const currentGame = ref<GameSnapshot | null>(null);
  const messages = ref<ChatMessageDto[]>([]);
  // Progress-only set of who has voted this voting round (secret ballot — the
  // backend never sends the target). Reset on phase change / elimination.
  const votedParticipantIds = ref<string[]>([]);
  // The tally from the most recent Eliminated event (full tally is revealed at
  // elimination time, per ADR 0004). Reset on a fresh snapshot load.
  const lastTally = ref<TallyDto | null>(null);
  const rouletteTiedIds = ref<string[]>([]);
  const survivors = ref<string[]>([]);
  const finished = ref(false);
  const connecting = ref(false);
  const phaseDeadline = ref<number | null>(null);

  const api = () => getApiContainer().game;
  const rt = (): IGameRealtime => getApiContainer().gameRealtime;

  const participants = computed<ParticipantDto[]>(() => currentGame.value?.participants ?? []);
  const phase = computed<GamePhase | null>(() => currentGame.value?.phase ?? null);
  const roundNumber = computed(() => currentGame.value?.roundNumber ?? 0);
  const bunkerCard = computed<BunkerCardDto | null>(() => currentGame.value?.bunkerCard ?? null);
  const bunkerCapacity = computed(() => currentGame.value?.bunkerCapacity ?? 0);
  const turnOrder = computed<string[]>(() => currentGame.value?.turnOrder ?? []);

  const me = computed<ParticipantDto | null>(() => participants.value.find((p) => p.isYou) ?? null);

  // Whose turn it is right now. Set directly from each TurnChanged event's
  // participantId (the saga names the player whose turn it is), and from the
  // snapshot on (re)load. The snapshot's `turnOrder` is only populated once the
  // saga reaches IntroDiscussion, so a client that loaded during
  // BunkerIntroduction has an empty `turnOrder` — deriving from `turnOrder[index]`
  // would leave this null for the whole game until a reload. Tracking the id the
  // hub sends avoids that dependency.
  const currentTurnParticipantId = ref<string | null>(null);

  const currentTurnParticipant = computed<ParticipantDto | null>(
    () => participants.value.find((p) => p.id === currentTurnParticipantId.value) ?? null,
  );

  const isMyTurn = computed(
    () => !!me.value && currentTurnParticipantId.value === me.value.id,
  );

  const myUnrevealedAttributeKinds = computed<string[]>(() => {
    const m = me.value;
    if (!m) return [];
    return m.attributes.filter((a) => !a.revealed).map((a) => a.kind);
  });

  // The store doesn't know VoteRound/TiedParticipantIds (not in the snapshot), so
  // the voting UI offers all non-eliminated others. On a revote round 2 the backend
  // rejects an out-of-tied-set target with a clear Failure → the view toasts it.
  const voteableParticipants = computed<ParticipantDto[]>(() =>
    participants.value.filter((p) => !p.eliminated && !p.isYou),
  );

  const aliveCount = computed(() => participants.value.filter((p) => !p.eliminated).length);

  function applySnapshot(snapshot: GameSnapshot): void {
    currentGame.value = snapshot;
    finished.value = snapshot.phase === 'Finished';
    // Transient per-round state isn't carried in the snapshot — reset on (re)load.
    votedParticipantIds.value = [];
    lastTally.value = null;
    rouletteTiedIds.value = [];
    phaseDeadline.value = null;
    currentTurnParticipantId.value =
      snapshot.currentTurnIndex >= 0 && snapshot.currentTurnIndex < snapshot.turnOrder.length
        ? snapshot.turnOrder[snapshot.currentTurnIndex] ?? null
        : null;
    survivors.value = finished.value
      ? snapshot.participants.filter((p) => !p.eliminated).map((p) => p.id)
      : [];
  }

  async function fetchGame(id: string): Promise<GameSnapshot> {
    const snapshot = await api().getGame(id);
    applySnapshot(snapshot);
    return snapshot;
  }

  async function fetchMessages(id: string): Promise<void> {
    messages.value = await api().getChatMessages(id);
  }

  async function revealAttribute(kind: string): Promise<void> {
    if (!currentGame.value) return;
    await api().revealAttribute(currentGame.value.id, { attributeKind: kind });
  }

  async function sendMessage(text: string): Promise<void> {
    if (!currentGame.value) return;
    await api().sendMessage(currentGame.value.id, { text });
  }

  async function vote(targetParticipantId: string): Promise<void> {
    if (!currentGame.value) return;
    await api().vote(currentGame.value.id, { targetParticipantId });
  }

  // Re-fetch the snapshot + chat after a transport reconnect so we close any gap
  // missed while the socket was down (and recover chat history on reload).
  let reconnectHandler: (() => void) | null = null;

  async function connectAndJoin(gameId: string): Promise<void> {
    connecting.value = true;
    try {
      await rt().connect();
      registerRealtimeHandlers();
      if (reconnectHandler) rt().offReconnected(reconnectHandler);
      reconnectHandler = () => {
        // Silent: a flaky connection shouldn't toast on every reconnect fetch.
        if (currentGame.value) {
          fetchGame(currentGame.value.id).catch(() => {});
          fetchMessages(currentGame.value.id).catch(() => {});
        }
      };
      rt().onReconnected(reconnectHandler);
      await rt().joinGame(gameId);
    } finally {
      connecting.value = false;
    }
  }

  async function disconnectRealtime(): Promise<void> {
    if (reconnectHandler) {
      rt().offReconnected(reconnectHandler);
      reconnectHandler = null;
    }
    if (currentGame.value) await rt().leaveGame(currentGame.value.id);
    await rt().disconnect();
  }

  function reset(): void {
    currentGame.value = null;
    messages.value = [];
    votedParticipantIds.value = [];
    lastTally.value = null;
    rouletteTiedIds.value = [];
    survivors.value = [];
    finished.value = false;
    phaseDeadline.value = null;
    currentTurnParticipantId.value = null;
  }

  function on<K extends GameEvent>(event: K, handler: GameEventHandler<K>): void {
    rt().on(event, handler);
  }

  function findParticipant(id: string): ParticipantDto | undefined {
    return participants.value.find((p) => p.id === id);
  }

  // Handlers close over this store's stable refs, and the store is a singleton, so
  // they only need to be registered once for the app lifetime. connect() re-wires
  // the dispatch wrappers to any new SignalR connection from these same handler
  // sets, so re-entries/reconnects keep receiving events. Without this guard, each
  // connectAndJoin adds another set of handlers and a single broadcast multiplies.
  let realtimeHandlersRegistered = false;

  function registerRealtimeHandlers(): void {
    if (realtimeHandlersRegistered) return;
    realtimeHandlersRegistered = true;

    rt().on('BunkerCardRevealed', ({ bunkerCard }) => {
      if (!currentGame.value) return;
      currentGame.value.bunkerCard = bunkerCard;
    });

    rt().on('PhaseChanged', ({ phase, roundNumber, phaseDurationSeconds }) => {
      if (!currentGame.value) return;
      currentGame.value.phase = phase;
      currentGame.value.roundNumber = roundNumber;
      votedParticipantIds.value = [];
      finished.value = phase === 'Finished';
      currentTurnParticipantId.value = null;
      phaseDeadline.value = phaseDurationSeconds > 0 ? Date.now() + phaseDurationSeconds * 1000 : null;
    });

    rt().on('TurnChanged', ({ participantId, phase, turnIndex, turnDurationSeconds }) => {
      if (!currentGame.value) return;
      currentGame.value.phase = phase;
      currentGame.value.currentTurnIndex = turnIndex;
      currentTurnParticipantId.value = participantId;
      phaseDeadline.value = turnDurationSeconds > 0 ? Date.now() + turnDurationSeconds * 1000 : null;
    });

    rt().on('AttributeRevealed', ({ participantId, kind, value }) => {
      const participant = findParticipant(participantId);
      if (!participant) return;
      const attr = participant.attributes.find((a) => a.kind === kind);
      if (attr) {
        attr.revealed = true;
        attr.value = value;
      } else {
        participant.attributes.push({ kind, value, revealed: true });
      }
    });

    rt().on('ChatMessageReceived', (msg) => {
      if (messages.value.some((m) => m.id === msg.id)) return;
      messages.value.push(msg);
    });

    rt().on('VoteCast', ({ participantId }) => {
      if (!votedParticipantIds.value.includes(participantId)) {
        votedParticipantIds.value = [...votedParticipantIds.value, participantId];
      }
    });

    rt().on('Eliminated', ({ participantId, tally }) => {
      const participant = findParticipant(participantId);
      if (participant) participant.eliminated = true;
      lastTally.value = tally;
      votedParticipantIds.value = [];
    });

    rt().on('RouletteStarted', ({ tiedParticipantIds }) => {
      rouletteTiedIds.value = tiedParticipantIds;
      if (currentGame.value) currentGame.value.phase = 'Roulette';
      currentTurnParticipantId.value = null;
      phaseDeadline.value = null;
    });

    rt().on('RouletteResult', ({ eliminatedId }) => {
      // The Eliminated event follows with the tally; just mark the sheet here.
      // Eliminated is idempotent on `eliminated`, so the follow-up is harmless.
      const participant = findParticipant(eliminatedId);
      if (participant) participant.eliminated = true;
    });

    rt().on('GameFinished', ({ survivorParticipantIds }) => {
      survivors.value = survivorParticipantIds;
      finished.value = true;
      if (currentGame.value) currentGame.value.phase = 'Finished';
      currentTurnParticipantId.value = null;
      phaseDeadline.value = null;
    });
  }

  return {
    currentGame,
    messages,
    votedParticipantIds,
    lastTally,
    rouletteTiedIds,
    survivors,
    finished,
    connecting,
    phaseDeadline,
    participants,
    phase,
    roundNumber,
    bunkerCard,
    bunkerCapacity,
    turnOrder,
    me,
    currentTurnParticipantId,
    currentTurnParticipant,
    isMyTurn,
    myUnrevealedAttributeKinds,
    voteableParticipants,
    aliveCount,
    fetchGame,
    fetchMessages,
    revealAttribute,
    sendMessage,
    vote,
    connectAndJoin,
    disconnectRealtime,
    reset,
    on,
  };
});