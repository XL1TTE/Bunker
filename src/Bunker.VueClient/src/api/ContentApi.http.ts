import type { CardPackPreview, PersonalityPresetPreview } from '@/types/content.types';
import { apiRequest, type AuthTokenProvider } from './http';
import type { IContentApi } from './IContentApi';

export class ContentApiHttp implements IContentApi {
  constructor(private readonly tokens: AuthTokenProvider) {}

  listCardPacks(): Promise<CardPackPreview[]> {
    return apiRequest(this.tokens, 'GET', '/content/packs');
  }

  listPersonalityPresets(): Promise<PersonalityPresetPreview[]> {
    return apiRequest(this.tokens, 'GET', '/content/personalities');
  }
}