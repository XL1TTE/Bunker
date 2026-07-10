import type { InjectionKey } from 'vue';
import type {
  BunkerCardDto,
  CardPackDto,
  CardTypeKey,
  CreateBunkerCardRequest,
  CreateCardPackRequest,
  CreateCardRequest,
  CreatePersonalityPresetRequest,
  PersonalityPresetDto,
  UnknownCard,
} from '@/types/admin.types';

export interface CardPage {
  total: number;
  cards: UnknownCard[];
}

export interface IContentAdminApi {
  // Character cards (7 types share one shape).
  listCards(type: CardTypeKey, skip: number, take: number): Promise<CardPage>;
  createCard(type: CardTypeKey, body: CreateCardRequest): Promise<UnknownCard>;
  updateCard(type: CardTypeKey, id: string, body: CreateCardRequest): Promise<UnknownCard>;
  deleteCard(id: string): Promise<void>;

  // Bunker cards (global canned content — not pack-scoped).
  listBunkerCards(): Promise<BunkerCardDto[]>;
  createBunkerCard(body: CreateBunkerCardRequest): Promise<BunkerCardDto>;
  updateBunkerCard(id: string, body: CreateBunkerCardRequest): Promise<BunkerCardDto>;
  deleteBunkerCard(id: string): Promise<void>;

  // Card packs (admin full list includes cardIds + generationPrompt).
  listPacksFull(): Promise<CardPackDto[]>;
  createPack(body: CreateCardPackRequest): Promise<CardPackDto>;
  updatePack(id: string, body: CreateCardPackRequest): Promise<CardPackDto>;
  deletePack(id: string): Promise<void>;

  // Personality presets (bot personalities).
  listPresets(): Promise<PersonalityPresetDto[]>;
  createPreset(body: CreatePersonalityPresetRequest): Promise<PersonalityPresetDto>;
  updatePreset(id: string, body: CreatePersonalityPresetRequest): Promise<PersonalityPresetDto>;
  deletePreset(id: string): Promise<void>;
}

export const ContentAdminApiKey: InjectionKey<IContentAdminApi> = Symbol('IContentAdminApi');