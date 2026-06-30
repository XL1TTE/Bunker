import type { InjectionKey } from 'vue';
import type {
  AddBotRequest,
  CreateLobbyRequest,
  LobbySnapshot,
  LobbySummary,
  SendMessageRequest,
  UpdateSettingsRequest,
} from '@/types/lobby.types';

export interface ILobbyApi {
  createLobby(request: CreateLobbyRequest): Promise<LobbySnapshot>;
  joinLobby(inviteCode: string): Promise<LobbySnapshot>;
  leaveLobby(lobbyId: string): Promise<void>;
  getLobby(lobbyId: string): Promise<LobbySnapshot>;
  listPublicLobbies(limit?: number, offset?: number): Promise<{ items: LobbySummary[]; total: number }>;
  updateSettings(lobbyId: string, request: UpdateSettingsRequest): Promise<LobbySnapshot>;
  addBot(lobbyId: string, request: AddBotRequest): Promise<LobbySnapshot>;
  removeBot(lobbyId: string, participantId: string): Promise<void>;
  kickParticipant(lobbyId: string, participantId: string): Promise<void>;
  toggleReadiness(lobbyId: string): Promise<LobbySnapshot>;
  startLobby(lobbyId: string): Promise<void>;
  sendMessage(lobbyId: string, request: SendMessageRequest): Promise<void>;
}

export const LobbyApiKey: InjectionKey<ILobbyApi> = Symbol('ILobbyApi');