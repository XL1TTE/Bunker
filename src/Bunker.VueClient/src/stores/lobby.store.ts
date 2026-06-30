import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { getApiContainer } from '@/api/register';
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
  const connecting = ref(false);
  const myAccountId = ref<string | null>(null);

  const api = () => getApiContainer().lobby;
  const rt = (): ILobbyRealtime => getApiContainer().realtime;

  const participants = computed(() => currentLobby.value?.participants ?? []);
  const hostParticipantId = computed(() => currentLobby.value?.hostParticipantId ?? null);
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

  async function fetchLobby(id: string): Promise<void> {
    const snapshot = await api().getLobby(id);
    applySnapshot(snapshot);
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

  async function leaveCurrent(): Promise<void> {
    if (!currentLobby.value) return;
    const id = currentLobby.value.id;
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
    await api().startLobby(currentLobby.value.id);
  }

  async function sendMessage(text: string): Promise<void> {
    if (!currentLobby.value) return;
    await api().sendMessage(currentLobby.value.id, { text });
  }

  async function connectAndJoin(lobbyId: string): Promise<void> {
    connecting.value = true;
    try {
      await rt().connect();
      registerRealtimeHandlers();
      await rt().joinLobby(lobbyId);
    } finally {
      connecting.value = false;
    }
  }

  async function disconnectRealtime(): Promise<void> {
    if (currentLobby.value) await rt().leaveLobby(currentLobby.value.id);
    await rt().disconnect();
  }

  function reset(): void {
    currentLobby.value = null;
    messages.value = [];
    destroyed.value = null;
    handoffGameId.value = null;
    myAccountId.value = null;
  }

  function setMyAccountId(accountId: string | null): void {
    myAccountId.value = accountId;
  }

  function on<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    rt().on(event, handler);
  }

  function registerRealtimeHandlers(): void {
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
      handoffGameId.value = gameSessionId;
    });
  }

  return {
    currentLobby,
    messages,
    summaries,
    summariesTotal,
    destroyed,
    handoffGameId: computed(() => handoffGameId.value),
    connecting,
    participants,
    hostParticipantId,
    isHost,
    findParticipant,
    fetchBrowser,
    fetchLobby,
    create,
    joinByCode,
    leaveCurrent,
    updateSettings,
    addBot,
    removeBot,
    kickParticipant,
    toggleReadiness,
    start,
    sendMessage,
    connectAndJoin,
    disconnectRealtime,
    on,
    reset,
    setMyAccountId,
  };
});