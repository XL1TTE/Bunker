import type { PlayerProfile } from '@/types/account.types';
import { SELF_PROFILE } from './seed-lobbies';
import type { IAccountApi } from '@/api/IAccountApi';

let currentProfile: PlayerProfile = { ...SELF_PROFILE };

export class AccountApiMock implements IAccountApi {
  async getMyProfile(): Promise<PlayerProfile> {
    await delay(80);
    return { ...currentProfile };
  }

  async getProfileById(id: string): Promise<PlayerProfile> {
    await delay(60);
    if (id === SELF_PROFILE.id) return { ...currentProfile };
    return {
      id,
      nickname: id === 'account-alice' ? 'Alice' : 'Bob',
      totalGames: 7,
      wins: 2,
      losses: 5,
    };
  }

  async updateMyNickname(nickname: string): Promise<PlayerProfile> {
    await delay(80);
    currentProfile = { ...currentProfile, nickname };
    return { ...currentProfile };
  }
}

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}