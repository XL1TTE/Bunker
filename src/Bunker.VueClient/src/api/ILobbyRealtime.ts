import type { InjectionKey, Ref } from 'vue';
import type { ChatMessage, LobbyDestroyedReason, LobbySnapshot } from '@/types/lobby.types';

export type RealtimeState = 'disconnected' | 'connecting' | 'connected';

export type LobbyEventMap = {
  ParticipantJoined: { participant: import('@/types/lobby.types').Participant };
  ParticipantLeft: { participantId: string };
  ParticipantKicked: { participantId: string; byHostId: string };
  BotAdded: { bot: import('@/types/lobby.types').Participant };
  BotRemoved: { participantId: string };
  SettingsChanged: { lobby: LobbySnapshot };
  ReadinessChanged: { participantId: string; status: 'Ready' | 'NotReady' };
  ChatMessageReceived: ChatMessage;
  LobbyDestroyed: { reason: LobbyDestroyedReason };
  HandoffStarted: { gameSessionId: string };
};

export type LobbyEvent = keyof LobbyEventMap;

export type LobbyEventHandler<K extends LobbyEvent> = (payload: LobbyEventMap[K]) => void;

export interface ILobbyRealtime {
  readonly state: Ref<RealtimeState>;
  connect(): Promise<void>;
  disconnect(): Promise<void>;
  joinLobby(lobbyId: string): Promise<void>;
  leaveLobby(lobbyId: string): Promise<void>;
  on<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void;
  off<K extends LobbyEvent>(event: K, handler: LobbyEventHandler<K>): void;
}

export const LobbyRealtimeKey: InjectionKey<ILobbyRealtime> = Symbol('ILobbyRealtime');