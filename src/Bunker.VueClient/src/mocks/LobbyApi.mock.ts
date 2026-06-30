import type {
  AddBotRequest,
  CreateLobbyRequest,
  LobbySnapshot,
  LobbySummary,
  SendMessageRequest,
  UpdateSettingsRequest,
} from '@/types/lobby.types';
import { SEED_LOBBY, SEED_LOBBY_SUMMARIES } from './seed-lobbies';
import type { ILobbyApi } from '@/api/ILobbyApi';

const lobbies = new Map<string, LobbySnapshot>([
  [SEED_LOBBY.id, structuredClone(SEED_LOBBY)],
]);

let inviteCounter = 1000;

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function makeInviteCode(): string {
  inviteCounter += 1;
  return `MOCK${inviteCounter}`;
}

export class LobbyApiMock implements ILobbyApi {
  async createLobby(request: CreateLobbyRequest): Promise<LobbySnapshot> {
    await delay(120);
    const id = `lobby-${Math.random().toString(36).slice(2, 8)}`;
    const lobby: LobbySnapshot = {
      id,
      inviteCode: makeInviteCode(),
      capacity: request.capacity,
      isPublic: request.isPublic,
      hostParticipantId: 'p-host-new',
      participants: [
        {
          id: 'p-host-new',
          nickname: 'You',
          role: 'Host',
          status: 'Ready',
          type: 'Player',
          accountId: 'account-self',
        },
      ],
      selectedPackIds: [...request.selectedPackIds],
    };
    lobbies.set(id, lobby);
    return structuredClone(lobby);
  }

  async joinLobby(inviteCode: string): Promise<LobbySnapshot> {
    await delay(100);
    const match = [...lobbies.values()].find((l) => l.inviteCode === inviteCode);
    if (!match) {
      throw new Error(`Lobby with code ${inviteCode} not found.`);
    }
    return structuredClone(match);
  }

  async leaveLobby(lobbyId: string): Promise<void> {
    await delay(60);
    lobbies.delete(lobbyId);
  }

  async getLobby(lobbyId: string): Promise<LobbySnapshot> {
    await delay(60);
    const lobby = lobbies.get(lobbyId);
    if (!lobby) {
      throw new Error(`Lobby ${lobbyId} not found.`);
    }
    return structuredClone(lobby);
  }

  async listPublicLobbies(): Promise<{ items: LobbySummary[]; total: number }> {
    await delay(80);
    return { items: SEED_LOBBY_SUMMARIES.map((s) => ({ ...s })), total: SEED_LOBBY_SUMMARIES.length };
  }

  async updateSettings(lobbyId: string, request: UpdateSettingsRequest): Promise<LobbySnapshot> {
    await delay(80);
    const lobby = lobbies.get(lobbyId);
    if (!lobby) throw new Error('Lobby not found.');
    if (request.capacity !== undefined) lobby.capacity = request.capacity;
    if (request.isPublic !== undefined) lobby.isPublic = request.isPublic;
    if (request.selectedPackIds !== undefined) lobby.selectedPackIds = [...request.selectedPackIds];
    return structuredClone(lobby);
  }

  async addBot(lobbyId: string, request: AddBotRequest): Promise<LobbySnapshot> {
    await delay(80);
    const lobby = lobbies.get(lobbyId);
    if (!lobby) throw new Error('Lobby not found.');
    lobby.participants.push({
      id: `b-${Math.random().toString(36).slice(2, 6)}`,
      nickname: request.nickname,
      role: 'Member',
      status: 'Ready',
      type: 'Bot',
      personalityPresetId: request.personalityPresetId,
    });
    return structuredClone(lobby);
  }

  async removeBot(lobbyId: string, participantId: string): Promise<void> {
    await delay(50);
    const lobby = lobbies.get(lobbyId);
    if (!lobby) return;
    lobby.participants = lobby.participants.filter(
      (p) => !(p.id === participantId && p.type === 'Bot'),
    );
  }

  async kickParticipant(lobbyId: string, participantId: string): Promise<void> {
    await delay(50);
    const lobby = lobbies.get(lobbyId);
    if (!lobby) return;
    lobby.participants = lobby.participants.filter((p) => p.id !== participantId);
  }

  async toggleReadiness(lobbyId: string): Promise<LobbySnapshot> {
    await delay(60);
    const lobby = lobbies.get(lobbyId);
    if (!lobby) throw new Error('Lobby not found.');
    const self = lobby.participants.find((p) => p.accountId === 'account-self');
    if (self) {
      self.status = self.status === 'Ready' ? 'NotReady' : 'Ready';
    }
    return structuredClone(lobby);
  }

  async startLobby(lobbyId: string): Promise<void> {
    await delay(150);
    console.debug('[mock] startLobby', lobbyId);
  }

  async sendMessage(lobbyId: string, request: SendMessageRequest): Promise<void> {
    await delay(30);
    console.debug('[mock] sendMessage', lobbyId, request.text);
  }
}