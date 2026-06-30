import { ref, type Ref } from 'vue';
import {
  type ILobbyRealtime,
  type LobbyEvent,
  type LobbyEventHandler,
  type LobbyEventMap,
  type RealtimeState,
} from '@/api/ILobbyRealtime';

export class LobbyRealtimeMock implements ILobbyRealtime {
  readonly state: Ref<RealtimeState> = ref('disconnected');

  private readonly handlers: Map<LobbyEvent, Set<(payload: unknown) => void>> = new Map();
  private joinedLobbyId: string | null = null;
  private simulationTimers: ReturnType<typeof setTimeout>[] = [];

  async connect(): Promise<void> {
    this.state.value = 'connecting';
    await delay(80);
    this.state.value = 'connected';
  }

  async disconnect(): Promise<void> {
    this.simulationTimers.forEach(clearTimeout);
    this.simulationTimers = [];
    this.joinedLobbyId = null;
    this.state.value = 'disconnected';
  }

  async joinLobby(lobbyId: string): Promise<void> {
    this.joinedLobbyId = lobbyId;
    this.scheduleSimulatedEvents(lobbyId);
  }

  async leaveLobby(lobbyId: string): Promise<void> {
    if (this.joinedLobbyId === lobbyId) {
      this.simulationTimers.forEach(clearTimeout);
      this.simulationTimers = [];
      this.joinedLobbyId = null;
    }
  }

  on<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    let set = this.handlers.get(event);
    if (!set) {
      set = new Set();
      this.handlers.set(event, set);
    }
    set.add(handler as (payload: unknown) => void);
  }

  off<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    this.handlers.get(event)?.delete(handler as (payload: unknown) => void);
  }

  private dispatch<K extends LobbyEvent>(event: K, payload: LobbyEventMap[K]): void {
    const handlers = this.handlers.get(event);
    if (!handlers) return;
    for (const h of handlers) {
      (h as (p: unknown) => void)(payload);
    }
  }

  private scheduleSimulatedEvents(lobbyId: string): void {
    const t1 = setTimeout(() => {
      this.dispatch('ChatMessageReceived', {
        id: crypto.randomUUID(),
        participantId: 'p-guest',
        nickname: 'Alice',
        text: 'Hey everyone, ready when you are.',
        sentAt: new Date().toISOString(),
      });
    }, 4000);
    const t2 = setTimeout(() => {
      this.dispatch('ReadinessChanged', { participantId: 'p-guest', status: 'Ready' });
    }, 8000);
    const t3 = setTimeout(() => {
      this.dispatch('ChatMessageReceived', {
        id: crypto.randomUUID(),
        participantId: 'b-1',
        nickname: 'Paranoiac',
        text: "Don't trust Alice.",
        sentAt: new Date().toISOString(),
      });
    }, 12000);
    this.simulationTimers.push(t1, t2, t3);
    void lobbyId;
  }
}

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}