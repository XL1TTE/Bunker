export type ParticipantRole = 'Host' | 'Member';
export type ParticipantStatus = 'Ready' | 'NotReady';
export type ParticipantType = 'Player' | 'Bot';

export interface Participant {
  id: string;
  nickname: string;
  role: ParticipantRole;
  status: ParticipantStatus;
  type: ParticipantType;
  accountId?: string;
  personalityPresetId?: string;
}

export interface LobbySnapshot {
  id: string;
  inviteCode: string;
  capacity: number;
  isPublic: boolean;
  hostParticipantId: string;
  participants: Participant[];
  selectedPackIds: string[];
}

export interface LobbySummary {
  id: string;
  inviteCode: string;
  capacity: number;
  currentPlayers: number;
  hostNickname: string;
  selectedPackIds: string[];
}

export interface ChatMessage {
  id: string;
  participantId: string;
  nickname: string;
  text: string;
  sentAt: string;
}

export type LobbyDestroyedReason = 'HostLeft' | 'CapacityChanged' | 'Kicked';

export interface CreateLobbyRequest {
  capacity: number;
  isPublic: boolean;
  selectedPackIds: string[];
}

export interface AddBotRequest {
  personalityPresetId: string;
  nickname: string;
}

export interface UpdateSettingsRequest {
  capacity?: number;
  isPublic?: boolean;
  selectedPackIds?: string[];
}

export interface SendMessageRequest {
  text: string;
}