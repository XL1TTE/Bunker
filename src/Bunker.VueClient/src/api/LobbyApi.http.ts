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

  // Join a public lobby straight from the browser list — no invite code (those
  // are private to the host now) and no password. Shares the /join route with
  // joinLobbyByPassword; a missing body means "open public lobby".
  joinLobbyById(lobbyId: string): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/join`);
  }

  joinLobbyByPassword(lobbyId: string, password: string): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/join`, { password });
  }

  async leaveLobby(lobbyId: string): Promise<void> {
    await apiRequest(this.tokens, 'POST', `/lobbies/${lobbyId}/leave`);
  }

  getLobby(lobbyId: string): Promise<LobbySnapshot> {
    return apiRequest(this.tokens, 'GET', `/lobbies/${lobbyId}`);
  }

  // Host-only: the invite code is no longer carried on the lobby snapshot, so
  // the host fetches it through this dedicated endpoint to share it.
  getInviteCode(lobbyId: string): Promise<{ inviteCode: string }> {
    return apiRequest(this.tokens, 'GET', `/lobbies/${lobbyId}/invite-code`);
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