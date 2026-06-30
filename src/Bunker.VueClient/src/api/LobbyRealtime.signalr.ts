import * as signalR from '@microsoft/signalr';
import { ref, type Ref } from 'vue';
import type { AuthTokenProvider } from './http';
import {
  type ILobbyRealtime,
  type LobbyEvent,
  type LobbyEventHandler,
  type RealtimeState,
} from './ILobbyRealtime';

export class LobbyRealtimeSignalR implements ILobbyRealtime {
  readonly state: Ref<RealtimeState> = ref('disconnected');

  private connection: signalR.HubConnection | null = null;
  private readonly handlers: Map<LobbyEvent, Set<(payload: unknown) => void>> = new Map();

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

    this.connection.onreconnected(() => {
      this.state.value = 'connected';
    });
    this.connection.onclose(() => {
      this.state.value = 'disconnected';
    });

    for (const event of Object.keys(this.handlers) as LobbyEvent[]) {
      this.connection.on(event, (payload: unknown) => this.dispatch(event, payload));
    }

    await this.connection.start();
    this.state.value = 'connected';
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    await this.connection.stop();
    this.connection = null;
    this.state.value = 'disconnected';
  }

  async joinLobby(lobbyId: string): Promise<void> {
    await this.connection?.invoke('JoinLobby', lobbyId);
  }

  async leaveLobby(lobbyId: string): Promise<void> {
    await this.connection?.invoke('LeaveLobby', lobbyId);
  }

  on<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    let set = this.handlers.get(event);
    if (!set) {
      set = new Set();
      this.handlers.set(event, set);
      this.connection?.on(event, (payload: unknown) => this.dispatch(event, payload));
    }
    set.add(handler as (payload: unknown) => void);
  }

  off<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void {
    this.handlers.get(event)?.delete(handler as (payload: unknown) => void);
  }

  private dispatch(event: LobbyEvent, payload: unknown): void {
    const handlers = this.handlers.get(event);
    if (!handlers) return;
    for (const h of handlers) {
      (h as (p: unknown) => void)(payload);
    }
  }
}