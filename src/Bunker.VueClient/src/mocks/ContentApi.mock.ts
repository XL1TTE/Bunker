import type { CardPackPreview, PersonalityPresetPreview } from '@/types/content.types';
import { SEED_PACKS, SEED_PRESETS } from './seed-catalog';
import type { IContentApi } from '@/api/IContentApi';

export class ContentApiMock implements IContentApi {
  async listCardPacks(): Promise<CardPackPreview[]> {
    await delay(60);
    return SEED_PACKS.map((p) => ({ ...p }));
  }

  async listPersonalityPresets(): Promise<PersonalityPresetPreview[]> {
    await delay(60);
    return SEED_PRESETS.map((p) => ({ ...p }));
  }
}

function delay(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}