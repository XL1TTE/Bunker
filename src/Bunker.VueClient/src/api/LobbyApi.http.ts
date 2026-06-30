import type {
  AddBotRequest,
  CreateLobbyRequest,
  LobbySnapshot,
  LobbySummary,
  SendMessageRequest,
  UpdateSettingsRequest,
} from '@/types/lobby.types';
import { apiRequest, type AuthTokenProvider } from './http';
import type { ILobbyApi } from './ILobbyApi';

export class LobbyApiHttp implements ILobbyApi {
  constructor(private readonly tokens: AuthTokenProvider) {}

  createLobby(request: CreateLobbyRequest): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'POST', '/lobbies', request);
  }

  joinLobby(inviteCode: string): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'POST', `/lobbies/${encodeURIComponent(inviteCode)}/join`);
  }

  async leaveLobby(lobbyId: string): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/leave`);
  }

  getLobby(lobbyId: string): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'GET', `/lobbies/${lobbyId}`);
  }

  listPublicLobbies(limit = 50, offset = 0): Promise<{ items: LobbySummary[]; total: number }> {
    return apiRequest(this.tokens, 'GET', `/lobbies?limit=${limit}&offset=${offset}`);
  }

  updateSettings(lobbyId: string, request: UpdateSettingsRequest): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'PATCH', `/lobbies/${lobbyId}/settings`, request);
  }

  addBot(lobbyId: string, request: AddBotRequest): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/bots`, request);
  }

  async removeBot(lobbyId: string, participantId: string): Promise<void> {
    await apiRequest(this.tokens, 'DELETE', `/lobbies/${lobbyId}/bots/${participantId}`);
  }

  async kickParticipant(lobbyId: string, participantId: string): Promise<void> {
    await apiRequest(this.tokens, 'DELETE', `/lobbies/${lobbyId}/participants/${participantId}`);
  }

  toggleReadiness(lobbyId: string): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/ready`);
  }

  async startLobby(lobbyId: string): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/start`);
  }

  async sendMessage(lobbyId: string, request: SendMessageRequest): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/messages`, request);
  }
}