import * as signalR from '@microsoft/signalr';
import { ref, type Ref } from 'vue';
import type { AuthTokenProvider } from './http';
import {
  type ILobbyRealtime,
  type LobbyEvent,
  type LobbyEventMap,
  type LobbyEventHandler,
  type RealtimeState,
} from './ILobbyRealtime';
import type {
  ChatMessage,
  LobbyDestroyedReason,
  LobbySnapshot,
  Participant,
} from '@/types/lobby.types';

// The backend hub (ILobbyHub) invokes each event with **positional args**:
//   ParticipantJoined(participant)            — 1 object arg
//   ParticipantLeft(participantId)            — 1 string arg
//   ParticipantKicked(participantId, byHostId)— 2 string args
//   ReadinessChanged(participantId, status)   — 2 string args
//   ...
// The frontend contract (LobbyEventMap) is a single wrapped payload object
// (e.g. { participant }, { participantId, byHostId }). These adapters convert
// the raw SignalR args into the payload shape the store handlers destructure.
type PayloadAdapter<K extends LobbyEvent> = (args: unknown[]) => LobbyEventMap[K];

const payloadAdapters: { [K in LobbyEvent]: PayloadAdapter<K> } = {
  ParticipantJoined: (args) => ({ participant: args[0] as Participant }),
  ParticipantLeft: (args) => ({ participantId: args[0] as string }),
  ParticipantKicked: (args) => ({ participantId: args[0] as string, byHostId: args[1] as string }),
  BotAdded: (args) => ({ bot: args[0] as Participant }),
  BotRemoved: (args) => ({ participantId: args[0] as string }),
  SettingsChanged: (args) => ({ lobby: args[0] as LobbySnapshot }),
  ReadinessChanged: (args) => ({
    participantId: args[0] as string,
    status: args[1] as 'Ready' | 'NotReady',
  }),
  ChatMessageReceived: (args) => args[0] as ChatMessage,
  LobbyDestroyed: (args) => ({ reason: args[0] as LobbyDestroyedReason }),
  HandoffStarted: (args) => ({ gameSessionId: args[0] as string }),
  GameStartFailed: (args) => ({ reason: args[0] as string }),
};

export class LobbyRealtimeSignalR implements ILobbyRealtime {
  readonly state: Ref<RealtimeState> = ref('disconnected');

  private connection: signalR.HubConnection | null = null;
  private readonly handlers: Map<LobbyEvent, Set<(payload: unknown) => void>> = new Map();
  private readonly reconnectedHandlers: Set<() => void> = new Set();
  // The lobby group we're currently subscribed to, so a reconnect can re-join it.
  private joinedLobbyId: string | null = null;

  constructor(private readonly tokens: AuthTokenProvider) {}

  async connect(): Promise<void> {
    if (this.connection) return;
    this.state.value = 'connecting';
    const token = await this.tokens.getAccessToken();
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/lobby`, {
        accessTokenFactory: () => token ?? '',
      })
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnected(async () => {
      this.state.value = 'connected';
      // The server drops group membership on disconnect, so re-subscribe before
      // notifying the store to re-fetch the snapshot.
      if (this.joinedLobbyId) {
        try {
          await this.connection?.invoke('JoinLobby', this.joinedLobbyId);
        } catch {
          // If re-join fails, the store's re-fetch will still surface state; the
          // next manual action will retry the group subscription via joinLobby.
        }
      }
      for (const handler of this.reconnectedHandlers) {
        try {
          handler();
        } catch {
          // A handler throwing must not break the others or the reconnect flow.
        }
      }
    });
    this.connection.onclose(() => {
      this.state.value = 'disconnected';
    });

    for (const event of Array.from(this.handlers.keys()) as LobbyEvent[]) {
      this.connection.on(event, (...args: unknown[]) => this.dispatch(event, args));
    }

    await this.connection.start();
    this.state.value = 'connected';
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    this.joinedLobbyId = null;
    await this.connection.stop();
    this.connection = null;
    this.state.value = 'disconnected';
  }

  async joinLobby(lobbyId: string): Promise<void> {
    // Skip if we're already subscribed to this lobby's group on the current
    // connection — a duplicate AddToGroupAsync could multiply deliveries, and
    // a re-mount shouldn't re-join. (Reconnect re-joins via onreconnected, not
    // here, and disconnect/leave clear joinedLobbyId so genuine re-joins run.)
    if (this.joinedLobbyId === lobbyId && this.connection) return;
    this.joinedLobbyId = lobbyId;
    await this.connection?.invoke('JoinLobby', lobbyId);
  }

  async leaveLobby(lobbyId: string): Promise<void> {
    if (this.joinedLobbyId === lobbyId) {
      this.joinedLobbyId = null;
    }
    await this.connection?.invoke('LeaveLobby', lobbyId);
  }

  onReconnected(handler: () => void): void {
    this.reconnectedHandlers.add(handler);
  }

  offReconnected(handler: () => void): void {
    this.reconnectedHandlers.delete(handler);
  }

  on<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    let set = this.handlers.get(event);
    if (!set) {
      set = new Set();
      this.handlers.set(event, set);
      this.connection?.on(event, (...args: unknown[]) => this.dispatch(event, args));
    }
    set.add(handler as (payload: unknown) => void);
  }

  off<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    this.handlers.get(event)?.delete(handler as (payload: unknown) => void);
  }

  private dispatch(event: LobbyEvent, args: unknown[]): void {
    const handlers = this.handlers.get(event);
    if (!handlers) return;
    const payload = payloadAdapters[event](args);
    for (const h of handlers) {
      (h as (p: unknown) => void)(payload);
    }
  }
}