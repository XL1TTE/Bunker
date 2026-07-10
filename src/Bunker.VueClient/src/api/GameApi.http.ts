import type {
  ChatMessageDto,
  GameSnapshot,
  RevealAttributeRequest,
  SendMessageRequest,
  VoteRequest,
} from '@/types/game.types';
import { apiRequest, type AuthTokenProvider } from './http';
import type { IGameApi } from './IGameApi';

export class GameApiHttp implements IGameApi {
  constructor(private readonly tokens: AuthTokenProvider) {}

  getGame(gameId: string): Promise<GameSnapshot> {
    return apiRequest(this.tokens, 'GET', `/game/${gameId}`);
  }

  getChatMessages(gameId: string): Promise<ChatMessageDto[]> {
    return apiRequest(this.tokens, 'GET', `/game/${gameId}/chat`);
  }

  async revealAttribute(gameId: string, request: RevealAttributeRequest): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/game/${gameId}/reveal`, request);
  }

  async sendMessage(gameId: string, request: SendMessageRequest): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/game/${gameId}/chat`, request);
  }

  async vote(gameId: string, request: VoteRequest): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/game/${gameId}/vote`, request);
  }
}