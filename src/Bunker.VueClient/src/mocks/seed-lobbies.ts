import type { LobbySnapshot } from '@/types/lobby.types';

const PLAYER_HOST_ID = 'p-host';
const PLAYER_GUEST_ID = 'p-guest';
const BOT_1_ID = 'b-1';

// Invite codes are private to the host, so they live in a separate map keyed by
// lobby id — not on the snapshots. The browser list never sees these; only the
// host's getInviteCode call and the typed-code join path read them.
export const SEED_INVITE_CODES: Record<string, string> = {
  'lobby-1': 'BUNK42',
  'lobby-2': 'GAMER7',
  'lobby-3': 'DEEP9X',
};

export const SEED_LOBBY: LobbySnapshot = {
  id: 'lobby-1',
  name: 'Bunker Beta',
  capacity: 8,
  isPublic: true,
  hostParticipantId: PLAYER_HOST_ID,
  state: 'WaitingForPlayers',
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

// Full snapshots for the other two browser-listed lobbies, so joining them by
// id from the browser works in the mock (the list shows all three).
export const SEED_LOBBY_2: LobbySnapshot = {
  id: 'lobby-2',
  name: 'Game Night',
  capacity: 6,
  isPublic: true,
  hostParticipantId: 'p-bob',
  state: 'WaitingForPlayers',
  participants: [
    { id: 'p-bob', nickname: 'Bob', role: 'Host', status: 'Ready', type: 'Player', accountId: 'account-bob' },
    { id: 'p-dan', nickname: 'Dan', role: 'Member', status: 'Ready', type: 'Player', accountId: 'account-dan' },
    { id: 'p-eve', nickname: 'Eve', role: 'Member', status: 'NotReady', type: 'Player', accountId: 'account-eve' },
    { id: 'b-bob-1', nickname: 'Stoic', role: 'Member', status: 'Ready', type: 'Bot', personalityPresetId: 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb' },
  ],
  selectedPackIds: ['11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222'],
};

export const SEED_LOBBY_3: LobbySnapshot = {
  id: 'lobby-3',
  name: 'Deep Dive',
  capacity: 10,
  isPublic: true,
  hostParticipantId: 'p-carol',
  state: 'WaitingForPlayers',
  participants: [
    { id: 'p-carol', nickname: 'Carol', role: 'Host', status: 'Ready', type: 'Player', accountId: 'account-carol' },
    { id: 'p-frank', nickname: 'Frank', role: 'Member', status: 'NotReady', type: 'Player', accountId: 'account-frank' },
  ],
  selectedPackIds: ['33333333-3333-3333-3333-333333333333'],
};

export const SEED_LOBBIES: LobbySnapshot[] = [SEED_LOBBY, SEED_LOBBY_2, SEED_LOBBY_3];

export const SEED_LOBBY_SUMMARIES = [
  {
    id: 'lobby-1',
    name: 'Bunker Beta',
    capacity: 8,
    currentPlayers: 3,
    hasPassword: false,
    hostNickname: 'You',
    selectedPackIds: ['11111111-1111-1111-1111-111111111111'],
  },
  {
    id: 'lobby-2',
    name: 'Game Night',
    capacity: 6,
    currentPlayers: 4,
    hasPassword: true,
    hostNickname: 'Bob',
    selectedPackIds: ['11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222'],
  },
  {
    id: 'lobby-3',
    name: 'Deep Dive',
    capacity: 10,
    currentPlayers: 2,
    hasPassword: false,
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