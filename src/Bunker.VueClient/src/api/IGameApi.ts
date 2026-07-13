import type { InjectionKey } from 'vue';
import type {
  ChatMessageDto,
  GameSnapshot,
  RevealAttributeRequest,
  SendMessageRequest,
  VoteRequest,
} from '@/types/game.types';

export interface IGameApi {
  getGame(gameId: string): Promise<GameSnapshot>;
  getChatMessages(gameId: string): Promise<ChatMessageDto[]>;
  revealAttribute(gameId: string, request: RevealAttributeRequest): Promise<void>;
  sendMessage(gameId: string, request: SendMessageRequest): Promise<void>;
  vote(gameId: string, request: VoteRequest): Promise<void>;
  leaveGame(gameId: string): Promise<void>;
}

export const GameApiKey: InjectionKey<IGameApi> = Symbol('IGameApi');