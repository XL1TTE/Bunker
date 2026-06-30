import type { InjectionKey } from 'vue';
import type { PlayerProfile } from '@/types/account.types';

export interface IAccountApi {
  getMyProfile(): Promise<PlayerProfile>;
  getProfileById(id: string): Promise<PlayerProfile>;
  updateMyNickname(nickname: string): Promise<PlayerProfile>;
}

export const AccountApiKey: InjectionKey<IAccountApi> = Symbol('IAccountApi');