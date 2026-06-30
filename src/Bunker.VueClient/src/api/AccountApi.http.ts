import type { PlayerProfile } from '@/types/account.types';
import { apiRequest, type AuthTokenProvider } from './http';
import type { IAccountApi } from './IAccountApi';

export class AccountApiHttp implements IAccountApi {
  constructor(private readonly tokens: AuthTokenProvider) {}

  getMyProfile(): Promise<PlayerProfile> {
    return apiRequest(this.tokens, 'GET', '/account/me');
  }

  getProfileById(id: string): Promise<PlayerProfile> {
    return apiRequest(this.tokens, 'GET', `/account/${id}`);
  }

  updateMyNickname(nickname: string): Promise<PlayerProfile> {
    return apiRequest(this.tokens, 'PATCH', '/account/me', { nickname });
  }
}