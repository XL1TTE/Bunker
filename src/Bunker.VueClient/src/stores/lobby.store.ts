import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { getApiContainer } from '@/api/register';
import { GAME_START_STEP_IDS, GAME_START_STEP_LABELS } from '@/components/lobby/game-start-steps';
import type { ILobbyRealtime, LobbyEvent, LobbyEventHandler } from '@/api/ILobbyRealtime';
import type {
  AddBotRequest,
  ChatMessage,
  CreateLobbyRequest,
  LobbyDestroyedReason,
  LobbySnapshot,
  LobbySummary,
  Participant,
  UpdateSettingsRequest,
} from '@/types/lobby.types';

export const useLobbyStore = defineStore('lobby', () => {
  const currentLobby = ref<LobbySnapshot | null>(null);
  const messages = ref<ChatMessage[]>([]);
  const summaries = ref<LobbySummary[]>([]);
  const summariesTotal = ref(0);
  const destroyed = ref<{ reason: LobbyDestroyedReason } | null>(null);
  const handoffGameId = ref<string | null>(null);
  const gameStartError = ref<string | null>(null);
  const connecting = ref(false);

  type StartStepStatus = 'pending' | 'started' | 'succeeded' | 'failed';
  interface StartStep {
    id: string;
    label: string;
    status: StartStepStatus;
    message: string | null;
  }
  const startSteps = ref<StartStep[]>(
    GAME_START_STEP_IDS.map((id) => ({ id, label: GAME_START_STEP_LABELS[id], status: 'pending' as StartStepStatus, message: null })),
  );
  const startInProgress = ref(false);
  const startFailed = ref(false);
  const startFailReason = ref<string | null>(null);

  function resetStartSteps(): void {
    startSteps.value = GAME_START_STEP_IDS.map((id) => ({ id, label: GAME_START_STEP_LABELS[id], status: 'pending' as StartStepStatus, message: null }));
  }

  function setStartStep(stepId: string, status: StartStepStatus, message: string | null): void {
    const idx = startSteps.value.findIndex((s) => s.id === stepId);
    if (idx < 0) return;
    for (let i = 0; i < idx; i++) {
      if (startSteps.value[i].status === 'pending') startSteps.value[i].status = 'succeeded';
    }
    startSteps.value[idx].status = status;
    if (status === 'failed') startSteps.value[idx].message = message;
  }

  function beginStart(): void {
    resetStartSteps();
    startFailed.value = false;
    startFailReason.value = null;
    setStartStep('validate-lobby', 'started', null);
    startInProgress.value = true;
  }

  function dismissStart(): void {
    startInProgress.value = false;
    resetStartSteps();
    startFailed.value = false;
    startFailReason.value = null;
  }
  const myAccountId = ref<string | null>(null);
  // The invite code for the current lobby. It's private to the host, so it's no
  // longer part of the snapshot — the host fetches it through a dedicated
  // endpoint and only when they're hosting. Null until fetched (or for guests).
  const inviteCode = ref<string | null>(null);

  const api = () => getApiContainer().lobby;
  const rt = (): ILobbyRealtime => getApiContainer().realtime;

  const participants = computed(() => currentLobby.value?.participants ?? []);
  const hostParticipantId = computed(() => currentLobby.value?.hostParticipantId ?? null);
  // The lobby we're currently a participant in, or null when we're in none.
  // The lobby browser uses this to render an "Enter" action for our own lobby
  // (even when it's full) and to prompt before joining a different one.
  const currentLobbyId = computed(() => currentLobby.value?.id ?? null);
  const isHost = computed(() => {
    const lobby = currentLobby.value;
    if (!lobby || !myAccountId.value) return false;
    const host = lobby.participants.find((p) => p.id === lobby.hostParticipantId);
    return host?.accountId === myAccountId.value;
  });

  function findParticipant(id: string): Participant | undefined {
    return participants.value.find((p) => p.id === id);
  }

  function applySnapshot(snapshot: LobbySnapshot): void {
    currentLobby.value = snapshot;
  }

  async function fetchBrowser(): Promise<void> {
    const data = await api().listPublicLobbies();
    summaries.value = data.items;
    summariesTotal.value = data.total;
  }

  async function fetchLobby(id: string): Promise<LobbySnapshot> {
    const snapshot = await api().getLobby(id);
    applySnapshot(snapshot);
    return snapshot;
  }

  async function create(request: CreateLobbyRequest): Promise<LobbySnapshot> {
    const snapshot = await api().createLobby(request);
    applySnapshot(snapshot);
    return snapshot;
  }

  async function joinByCode(code: string): Promise<LobbySnapshot> {
    const snapshot = await api().joinLobby(code);
    applySnapshot(snapshot);
    return snapshot;
  }

  // Join a public lobby listed in the browser — by id, no code (codes are
  // private to the host) and no password.
  async function joinById(lobbyId: string): Promise<LobbySnapshot> {
    const snapshot = await api().joinLobbyById(lobbyId);
    applySnapshot(snapshot);
    return snapshot;
  }

  async function joinByPassword(lobbyId: string, password: string): Promise<LobbySnapshot> {
    const snapshot = await api().joinLobbyByPassword(lobbyId, password);
    applySnapshot(snapshot);
    return snapshot;
  }

  // Host-only: fetch the current lobby's invite code so it can be shared. Only
  // meaningful for the host; guests have no business calling it (and the
  // backend rejects non-hosts).
  async function fetchInviteCode(): Promise<void> {
    if (!currentLobby.value) return;
    const { inviteCode: code } = await api().getInviteCode(currentLobby.value.id);
    inviteCode.value = code;
  }

  async function leaveCurrent(): Promise<void> {
    if (!currentLobby.value) return;
    const id = currentLobby.value.id;
    if (reconnectHandler) {
      rt().offReconnected(reconnectHandler);
      reconnectHandler = null;
    }
    await rt().leaveLobby(id);
    await rt().disconnect();
    await api().leaveLobby(id);
    reset();
  }

  async function updateSettings(request: UpdateSettingsRequest): Promise<void> {
    if (!currentLobby.value) return;
    applySnapshot(await api().updateSettings(currentLobby.value.id, request));
  }

  async function addBot(request: AddBotRequest): Promise<void> {
    if (!currentLobby.value) return;
    applySnapshot(await api().addBot(currentLobby.value.id, request));
  }

  async function removeBot(participantId: string): Promise<void> {
    if (!currentLobby.value) return;
    await api().removeBot(currentLobby.value.id, participantId);
    if (currentLobby.value) {
      currentLobby.value.participants = currentLobby.value.participants.filter(
        (p) => p.id !== participantId,
      );
    }
  }

  async function kickParticipant(participantId: string): Promise<void> {
    if (!currentLobby.value) return;
    await api().kickParticipant(currentLobby.value.id, participantId);
    if (currentLobby.value) {
      currentLobby.value.participants = currentLobby.value.participants.filter(
        (p) => p.id !== participantId,
      );
    }
  }

  async function toggleReadiness(): Promise<void> {
    if (!currentLobby.value) return;
    applySnapshot(await api().toggleReadiness(currentLobby.value.id));
  }

  async function start(): Promise<void> {
    if (!currentLobby.value) return;
    try {
      await api().startLobby(currentLobby.value.id);
    } catch (e) {
      const message = (e as { message?: string } | undefined)?.message ?? 'Could not start the game.';
      setStartStep('validate-lobby', 'failed', message);
      startFailed.value = true;
      startFailReason.value = message;
    }
  }

  async function sendMessage(text: string): Promise<void> {
    if (!currentLobby.value) return;
    await api().sendMessage(currentLobby.value.id, { text });
  }

  // Re-fetch the lobby after a transport reconnect so we close any gap missed
  // while the socket was down. Registered per connectAndJoin, cleared on disconnect.
  let reconnectHandler: (() => void) | null = null;

  async function connectAndJoin(lobbyId: string): Promise<void> {
    connecting.value = true;
    try {
      await rt().connect();
      registerRealtimeHandlers();
      if (reconnectHandler) rt().offReconnected(reconnectHandler);
      reconnectHandler = () => {
        // Silent: a flaky connection shouldn't toast on every reconnect fetch.
        if (currentLobby.value) {
          fetchLobby(currentLobby.value.id).catch(() => {});
        }
      };
      rt().onReconnected(reconnectHandler);
      await rt().joinLobby(lobbyId);
    } finally {
      connecting.value = false;
    }
  }

  async function disconnectRealtime(): Promise<void> {
    if (reconnectHandler) {
      rt().offReconnected(reconnectHandler);
      reconnectHandler = null;
    }
    if (currentLobby.value) await rt().leaveLobby(currentLobby.value.id);
    await rt().disconnect();
  }

  function reset(): void {
    currentLobby.value = null;
    messages.value = [];
    destroyed.value = null;
    handoffGameId.value = null;
    gameStartError.value = null;
    myAccountId.value = null;
    inviteCode.value = null;
    startInProgress.value = false;
    startFailed.value = false;
    startFailReason.value = null;
    resetStartSteps();
  }

  function setMyAccountId(accountId: string | null): void {
    myAccountId.value = accountId;
  }

  function on<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    rt().on(event, handler);
  }

  // Handlers close over this store's stable refs (currentLobby, messages, …),
  // and the store is a singleton, so they only need to be registered once for
  // the app lifetime. connect() re-wires the dispatch wrappers to any new
  // SignalR connection from these same handler sets, so re-entries/reconnects
  // keep receiving events. Without this guard, each connectAndJoin adds another
  // set of handlers and a single broadcast multiplies (e.g. 3 chat copies).
  let realtimeHandlersRegistered = false;

  function registerRealtimeHandlers(): void {
    if (realtimeHandlersRegistered) return;
    realtimeHandlersRegistered = true;

    rt().on('ParticipantJoined', ({ participant }) => {
      if (!currentLobby.value) return;
      if (!currentLobby.value.participants.some((p) => p.id === participant.id)) {
        currentLobby.value.participants.push(participant);
      }
    });
    rt().on('ParticipantLeft', ({ participantId }) => {
      if (!currentLobby.value) return;
      currentLobby.value.participants = currentLobby.value.participants.filter(
        (p) => p.id !== participantId,
      );
    });
    rt().on('ParticipantKicked', ({ participantId }) => {
      if (!currentLobby.value) return;
      currentLobby.value.participants = currentLobby.value.participants.filter(
        (p) => p.id !== participantId,
      );
    });
    rt().on('BotAdded', ({ bot }) => {
      if (!currentLobby.value) return;
      if (!currentLobby.value.participants.some((p) => p.id === bot.id)) {
        currentLobby.value.participants.push(bot);
      }
    });
    rt().on('BotRemoved', ({ participantId }) => {
      if (!currentLobby.value) return;
      currentLobby.value.participants = currentLobby.value.participants.filter(
        (p) => p.id !== participantId,
      );
    });
    rt().on('SettingsChanged', ({ lobby }) => {
      applySnapshot(lobby);
    });
    rt().on('ReadinessChanged', ({ participantId, status }) => {
      if (!currentLobby.value) return;
      const p = findParticipant(participantId);
      if (p) p.status = status;
    });
    rt().on('ChatMessageReceived', (msg) => {
      messages.value.push(msg);
    });
    rt().on('LobbyDestroyed', ({ reason }) => {
      destroyed.value = { reason };
    });
    rt().on('HandoffStarted', ({ gameSessionId }) => {
      for (const s of startSteps.value) {
        if (s.status !== 'failed') s.status = 'succeeded';
      }
      handoffGameId.value = gameSessionId;
      gameStartError.value = null;
    });
    rt().on('GameStartFailed', ({ reason }) => {
      gameStartError.value = reason;
      startFailed.value = true;
      startFailReason.value = reason;
      if (!startSteps.value.some((s) => s.status === 'failed')) {
        for (let i = startSteps.value.length - 1; i >= 0; i--) {
          const s = startSteps.value[i];
          if (s.status === 'started' || s.status === 'pending') {
            s.status = 'failed';
            s.message = reason;
            break;
          }
        }
      }
    });
    rt().on('GameStartProgress', ({ step, status, message }) => {
      startInProgress.value = true;
      const mapped: StartStepStatus =
        status === 'Started' ? 'started' : status === 'Succeeded' ? 'succeeded' : 'failed';
      setStartStep(step, mapped, message);
      if (mapped === 'failed') {
        startFailed.value = true;
        startFailReason.value = message;
      }
    });
  }

  return {
    currentLobby,
    messages,
    summaries,
    summariesTotal,
    destroyed,
    handoffGameId: computed(() => handoffGameId.value),
    gameStartError: computed(() => gameStartError.value),
    startSteps: computed(() => startSteps.value),
    startInProgress: computed(() => startInProgress.value),
    startFailed: computed(() => startFailed.value),
    startFailReason: computed(() => startFailReason.value),
    connecting,
    participants,
    hostParticipantId,
    currentLobbyId,
    isHost,
    inviteCode: computed(() => inviteCode.value),
    findParticipant,
    fetchBrowser,
    fetchLobby,
    fetchInviteCode,
    create,
    joinByCode,
    joinById,
    joinByPassword,
    leaveCurrent,
    updateSettings,
    addBot,
    removeBot,
    kickParticipant,
    toggleReadiness,
    start,
    beginStart,
    dismissStart,
    sendMessage,
    connectAndJoin,
    disconnectRealtime,
    on,
    reset,
    setMyAccountId,
  };
});