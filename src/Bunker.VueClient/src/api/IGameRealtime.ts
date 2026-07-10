import type { InjectionKey, Ref } from 'vue';
import type {
  BunkerCardDto,
  ChatMessageDto,
  GamePhase,
  TallyDto,
} from '@/types/game.types';

export type RealtimeState = 'disconnected' | 'connecting' | 'connected';

// Mirrors the backend IGameHub event surface (Bunker.GameService/Hubs/GameHub.cs).
// Each event is invoked by the server with **positional args**; the SignalR impl's
// payloadAdapters convert those into the single wrapped payload object the store
// handlers destructure (same pattern as the lobby realtime layer).
export type GameEventMap = {
  BunkerCardRevealed: { bunkerCard: BunkerCardDto };
  PhaseChanged: { phase: GamePhase; roundNumber: number };
  TurnChanged: { participantId: string; phase: GamePhase; turnIndex: number };
  AttributeRevealed: { participantId: string; kind: string; value: string };
  // Single object arg — unwrapped, like the lobby's ChatMessageReceived.
  ChatMessageReceived: ChatMessageDto;
  VoteCast: { participantId: string };
  Eliminated: { participantId: string; tally: TallyDto };
  RouletteStarted: { tiedParticipantIds: string[] };
  RouletteResult: { eliminatedId: string };
  GameFinished: { survivorParticipantIds: string[] };
};

export type GameEvent = keyof GameEventMap;

export type GameEventHandler<K extends GameEvent> = (payload: GameEventMap[K]) => void;

export interface IGameRealtime {
  readonly state: Ref<RealtimeState>;
  connect(): Promise<void>;
  disconnect(): Promise<void>;
  joinGame(gameId: string): Promise<void>;
  leaveGame(gameId: string): Promise<void>;
  on<K extends GameEvent>(event: K, handler: GameEventHandler<K>): void;
  off<K extends GameEvent>(event: K, handler: GameEventHandler<K>): void;
  /**
   * Fired after the transport reconnects (and has re-joined its game group).
   * Lets the store re-fetch the snapshot + chat history to close any gap missed
   * while disconnected.
   */
  onReconnected(handler: () => void): void;
  offReconnected(handler: () => void): void;
}

export const GameRealtimeKey: InjectionKey<IGameRealtime> = Symbol('IGameRealtime');