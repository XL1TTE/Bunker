import type { LobbySnapshot } from '@/types/lobby.types';

const PLAYER_HOST_ID = 'p-host';
const PLAYER_GUEST_ID = 'p-guest';
const BOT_1_ID = 'b-1';

export const SEED_LOBBY: LobbySnapshot = {
  id: 'lobby-1',
  inviteCode: 'BUNK42',
  capacity: 8,
  isPublic: true,
  hostParticipantId: PLAYER_HOST_ID,
  participants: [
    {
      id: PLAYER_HOST_ID,
      nickname: 'You',
      role: 'Host',
      status: 'Ready',
      type: 'Player',
      accountId: 'account-self',
    },
    {
      id: PLAYER_GUEST_ID,
      nickname: 'Alice',
      role: 'Member',
      status: 'NotReady',
      type: 'Player',
      accountId: 'account-alice',
    },
    {
      id: BOT_1_ID,
      nickname: 'Paranoiac',
      role: 'Member',
      status: 'Ready',
      type: 'Bot',
      personalityPresetId: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
    },
  ],
  selectedPackIds: ['11111111-1111-1111-1111-111111111111'],
};

export const SEED_LOBBY_SUMMARIES = [
  {
    id: 'lobby-1',
    inviteCode: 'BUNK42',
    capacity: 8,
    currentPlayers: 3,
    hostNickname: 'You',
    selectedPackIds: ['11111111-1111-1111-1111-111111111111'],
  },
  {
    id: 'lobby-2',
    inviteCode: 'GAMER',
    capacity: 6,
    currentPlayers: 4,
    hostNickname: 'Bob',
    selectedPackIds: ['11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222'],
  },
  {
    id: 'lobby-3',
    inviteCode: 'DEEP9',
    capacity: 10,
    currentPlayers: 2,
    hostNickname: 'Carol',
    selectedPackIds: ['33333333-3333-3333-3333-333333333333'],
  },
];

export const SELF_PROFILE = {
  id: 'account-self',
  nickname: 'You',
  totalGames: 12,
  wins: 4,
  losses: 8,
};