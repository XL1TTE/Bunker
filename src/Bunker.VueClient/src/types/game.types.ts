// Mirrors the backend GameService transfer DTOs verbatim (camelCase — the pipeline
// camelCases PascalCase C# records, same as the lobby contract). See
// `Transfers/Game/GameSnapshot.cs` + `Transfers/Game/ChatDtos.cs` in Bunker.GameService.

export type GamePhase =
  | 'BunkerIntroduction'
  | 'IntroDiscussion'
  | 'Reveal'
  | 'Discussion'
  | 'DiscussionClosing'
  | 'Voting'
  | 'Roulette'
  | 'Finished';

export type ParticipantType = 'Player' | 'Bot';

export interface AttributeDto {
  kind: string;
  value: string;
  revealed: boolean;
}

export interface ParticipantDto {
  id: string;
  nickname: string;
  type: ParticipantType;
  eliminated: boolean;
  isYou: boolean;
  attributes: AttributeDto[];
}

export interface BunkerCardDto {
  id: string;
  catastrophe: string;
  survivalDuration: string;
  bunkerEnvironment: string;
}

export interface GameSnapshot {
  id: string;
  roundNumber: number;
  phase: GamePhase;
  currentTurnIndex: number;
  bunkerCapacity: number;
  bunkerCard: BunkerCardDto;
  participants: ParticipantDto[];
  // Shuffled participant ids, built once in the GameSaga and reused every phase.
  // The frontend maps `currentTurnIndex -> turnOrder[index] -> participant.id` to
  // know whose turn it is on page reload (before any TurnChanged event arrives).
  turnOrder: string[];
}

export interface ChatMessageDto {
  id: string;
  participantId: string;
  nickname: string;
  text: string;
  sentAt: string;
}

export interface VoteTallyEntry {
  participantId: string;
  count: number;
}

export interface TallyDto {
  entries: VoteTallyEntry[];
  abstains: number;
}

export interface RevealAttributeRequest {
  attributeKind: string;
}

export interface SendMessageRequest {
  text: string;
}

export interface VoteRequest {
  targetParticipantId: string;
}