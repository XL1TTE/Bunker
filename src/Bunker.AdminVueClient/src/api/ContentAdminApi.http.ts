import { apiRequest, type AuthTokenProvider } from './http';
import type { IContentAdminApi, CardPage } from './IContentAdminApi';
import type {
  BunkerCardDto,
  BunkerCardListResponse,
  BunkerCardResponse,
  CardListResponse,
  CardPackDto,
  CardTypeKey,
  CreateBunkerCardRequest,
  CreateCardPackRequest,
  CreateCardRequest,
  CreatePersonalityPresetRequest,
  PackListResponse,
  PackResponse,
  PersonalityPresetDto,
  PresetListResponse,
  PresetResponse,
  UnknownCard,
} from '@/types/admin.types';

// Real-only HTTP impl over the shared fetch client. The admin panel talks to ContentService
// through the gateway (VITE_API_BASE_URL), so every path is /content/*.
//
// Path quirks mirrored from the backend:
//   - bunker-cards / packs / bots list+create roots have a trailing slash.
//   - the full pack list is /content/packs/full (the root /content/packs/ is the public preview).
//   - card list endpoints are paginated (skip/take); the others are unpaginated.
// Response payloads are wrapped in single-field structs ({ card }, { cards }, { pack },
// { packs }, { preset }, { presets }) — unwrapped here so callers get the bare arrays/objects.
export class ContentAdminApiHttp implements IContentAdminApi {
  constructor(private readonly tokens: AuthTokenProvider) {}

  // --- Cards ---

  async listCards(type: CardTypeKey, skip: number, take: number): Promise<CardPage> {
    const res = await apiRequest<CardListResponse>(
      this.tokens,
      'GET',
      `/content/cards/${type}?skip=${skip}&take=${take}`,
    );
    return { total: res.total, cards: res.cards };
  }

  async createCard(type: CardTypeKey, body: CreateCardRequest): Promise<UnknownCard> {
    const res = await apiRequest<{ card: UnknownCard }>(this.tokens, 'POST', `/content/cards/${type}`, body);
    return res.card;
  }

  async updateCard(type: CardTypeKey, id: string, body: CreateCardRequest): Promise<UnknownCard> {
    const res = await apiRequest<{ card: UnknownCard }>(this.tokens, 'PUT', `/content/cards/${type}/${id}`, body);
    return res.card;
  }

  async deleteCard(id: string): Promise<void> {
    await apiRequest<void>(this.tokens, 'DELETE', `/content/cards/${id}`);
  }

  // --- Bunker cards ---

  async listBunkerCards(): Promise<BunkerCardDto[]> {
    const res = await apiRequest<BunkerCardListResponse>(this.tokens, 'GET', '/content/bunker-cards/');
    return res.cards;
  }

  async createBunkerCard(body: CreateBunkerCardRequest): Promise<BunkerCardDto> {
    const res = await apiRequest<BunkerCardResponse>(this.tokens, 'POST', '/content/bunker-cards/', body);
    return res.card;
  }

  async updateBunkerCard(id: string, body: CreateBunkerCardRequest): Promise<BunkerCardDto> {
    const res = await apiRequest<BunkerCardResponse>(this.tokens, 'PUT', `/content/bunker-cards/${id}`, body);
    return res.card;
  }

  async deleteBunkerCard(id: string): Promise<void> {
    await apiRequest<void>(this.tokens, 'DELETE', `/content/bunker-cards/${id}`);
  }

  // --- Card packs ---

  async listPacksFull(): Promise<CardPackDto[]> {
    const res = await apiRequest<PackListResponse>(this.tokens, 'GET', '/content/packs/full');
    return res.packs;
  }

  async createPack(body: CreateCardPackRequest): Promise<CardPackDto> {
    const res = await apiRequest<PackResponse>(this.tokens, 'POST', '/content/packs/', body);
    return res.pack;
  }

  async updatePack(id: string, body: CreateCardPackRequest): Promise<CardPackDto> {
    const res = await apiRequest<PackResponse>(this.tokens, 'PUT', `/content/packs/${id}`, body);
    return res.pack;
  }

  async deletePack(id: string): Promise<void> {
    await apiRequest<void>(this.tokens, 'DELETE', `/content/packs/${id}`);
  }

  // --- Personality presets ---

  async listPresets(): Promise<PersonalityPresetDto[]> {
    const res = await apiRequest<PresetListResponse>(this.tokens, 'GET', '/content/bots/');
    return res.presets;
  }

  async createPreset(body: CreatePersonalityPresetRequest): Promise<PersonalityPresetDto> {
    const res = await apiRequest<PresetResponse>(this.tokens, 'POST', '/content/bots/', body);
    return res.preset;
  }

  async updatePreset(id: string, body: CreatePersonalityPresetRequest): Promise<PersonalityPresetDto> {
    const res = await apiRequest<PresetResponse>(this.tokens, 'PUT', `/content/bots/${id}`, body);
    return res.preset;
  }

  async deletePreset(id: string): Promise<void> {
    await apiRequest<void>(this.tokens, 'DELETE', `/content/bots/${id}`);
  }
}