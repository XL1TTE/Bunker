import * as signalR from '@microsoft/signalr';
import { ref, type Ref } from 'vue';
import type { AuthTokenProvider } from './http';
import {
  type IGameRealtime,
  type GameEvent,
  type GameEventMap,
  type GameEventHandler,
  type RealtimeState,
} from './IGameRealtime';
import type {
  BunkerCardDto,
  ChatMessageDto,
  GamePhase,
  TallyDto,
} from '@/types/game.types';

// The backend hub (IGameHub) invokes each event with **positional args**:
//   BunkerCardRevealed(bunkerCard)                      — 1 object arg
//   PhaseChanged(phase, roundNumber)                    — 2 args
//   TurnChanged(participantId, phase, turnIndex)        — 3 args
//   AttributeRevealed(participantId, kind, value)       — 3 string args
//   ChatMessageReceived(message)                        — 1 object arg
//   VoteCast(participantId)                             — 1 string arg
//   Eliminated(participantId, tally)                    — 1 string + 1 object
//   RouletteStarted(tiedParticipantIds)                 — 1 array arg
//   RouletteResult(eliminatedId)                        — 1 string arg
//   GameFinished(survivorParticipantIds)                — 1 array arg
// The frontend contract (GameEventMap) is a single wrapped payload object (or the
// raw object for ChatMessageReceived). These adapters convert the raw SignalR args
// into the payload shape the store handlers destructure.
type PayloadAdapter<K extends GameEvent> = (args: unknown[]) => GameEventMap[K];

const payloadAdapters: { [K in GameEvent]: PayloadAdapter<K> } = {
  BunkerCardRevealed: (args) => ({ bunkerCard: args[0] as BunkerCardDto }),
  PhaseChanged: (args) => ({
    phase: args[0] as GamePhase,
    roundNumber: args[1] as number,
  }),
  TurnChanged: (args) => ({
    participantId: args[0] as string,
    phase: args[1] as GamePhase,
    turnIndex: args[2] as number,
  }),
  AttributeRevealed: (args) => ({
    participantId: args[0] as string,
    kind: args[1] as string,
    value: args[2] as string,
  }),
  ChatMessageReceived: (args) => args[0] as ChatMessageDto,
  VoteCast: (args) => ({ participantId: args[0] as string }),
  Eliminated: (args) => ({
    participantId: args[0] as string,
    tally: args[1] as TallyDto,
  }),
  RouletteStarted: (args) => ({ tiedParticipantIds: args[0] as string[] }),
  RouletteResult: (args) => ({ eliminatedId: args[0] as string }),
  GameFinished: (args) => ({ survivorParticipantIds: args[0] as string[] }),
};

export class GameRealtimeSignalR implements IGameRealtime {
  readonly state: Ref<RealtimeState> = ref('disconnected');

  private connection: signalR.HubConnection | null = null;
  private readonly handlers: Map<GameEvent, Set<(payload: unknown) => void>> = new Map();
  private readonly reconnectedHandlers: Set<() => void> = new Set();
  // The game group we're currently subscribed to, so a reconnect can re-join it.
  private joinedGameId: string | null = null;

  constructor(private readonly tokens: AuthTokenProvider) {}

  async connect(): Promise<void> {
    if (this.connection) return;
    this.state.value = 'connecting';
    const token = await this.tokens.getAccessToken();
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_BASE_URL}/hubs/game`, {
        accessTokenFactory: () => token ?? '',
      })
      .withAutomaticReconnect()
      .build();

    this.connection.onreconnected(async () => {
      this.state.value = 'connected';
      // The server drops group membership on disconnect, so re-subscribe before
      // notifying the store to re-fetch the snapshot + chat history.
      if (this.joinedGameId) {
        try {
          await this.connection?.invoke('JoinGame', this.joinedGameId);
        } catch {
          // If re-join fails, the store's re-fetch will still surface state; the
          // next manual action will retry the group subscription via joinGame.
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

    for (const event of Array.from(this.handlers.keys()) as GameEvent[]) {
      this.connection.on(event, (...args: unknown[]) => this.dispatch(event, args));
    }

    await this.connection.start();
    this.state.value = 'connected';
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    this.joinedGameId = null;
    await this.connection.stop();
    this.connection = null;
    this.state.value = 'disconnected';
  }

  async joinGame(gameId: string): Promise<void> {
    // Skip if we're already subscribed to this game's group on the current
    // connection — a duplicate AddToGroupAsync could multiply deliveries, and
    // a re-mount shouldn't re-join. (Reconnect re-joins via onreconnected, not
    // here, and disconnect/leave clear joinedGameId so genuine re-joins run.)
    if (this.joinedGameId === gameId && this.connection) return;
    this.joinedGameId = gameId;
    await this.connection?.invoke('JoinGame', gameId);
  }

  async leaveGame(gameId: string): Promise<void> {
    if (this.joinedGameId === gameId) {
      this.joinedGameId = null;
    }
    await this.connection?.invoke('LeaveGame', gameId);
  }

  onReconnected(handler: () => void): void {
    this.reconnectedHandlers.add(handler);
  }

  offReconnected(handler: () => void): void {
    this.reconnectedHandlers.delete(handler);
  }

  on<K extends GameEvent>(event: K, handler: GameEventHandler<K>): void {
    let set = this.handlers.get(event);
    if (!set) {
      set = new Set();
      this.handlers.set(event, set);
      this.connection?.on(event, (...args: unknown[]) => this.dispatch(event, args));
    }
    set.add(handler as (payload: unknown) => void);
  }

  off<K extends GameEvent>(event: K, handler: GameEventHandler<K>): void {
    this.handlers.get(event)?.delete(handler as (payload: unknown) => void);
  }

  private dispatch(event: GameEvent, args: unknown[]): void {
    const handlers = this.handlers.get(event);
    if (!handlers) return;
    const payload = payloadAdapters[event](args);
    for (const h of handlers) {
      (h as (p: unknown) => void)(payload);
    }
  }
}