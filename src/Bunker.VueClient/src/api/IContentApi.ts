import type { InjectionKey } from 'vue';
import type { CardPackPreview, PersonalityPresetPreview } from '@/types/content.types';

export interface IContentApi {
  listCardPacks(): Promise<CardPackPreview[]>;
  listPersonalityPresets(): Promise<PersonalityPresetPreview[]>;
}

export const ContentApiKey: InjectionKey<IContentApi> = Symbol('IContentApi');