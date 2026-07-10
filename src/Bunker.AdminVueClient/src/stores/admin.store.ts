import { defineStore } from 'pinia';
import { ref } from 'vue';
import { getApiContainer } from '@/api/register';
import { CARD_TYPES, type CardTypeKey } from '@/types/admin.types';
import type {
  BunkerCardDto,
  CardPackDto,
  CreateBunkerCardRequest,
  CreateCardPackRequest,
  CreateCardRequest,
  CreatePersonalityPresetRequest,
  PersonalityPresetDto,
  UnknownCard,
} from '@/types/admin.types';

const PAGE_SIZE = 20;

export interface DashboardStats {
  cardCounts: Record<string, number>;
  bunkerCount: number;
  packCount: number;
  presetCount: number;
}

export interface CardListState {
  type: CardTypeKey;
  total: number;
  cards: UnknownCard[];
  skip: number;
  take: number;
}

export const useAdminStore = defineStore('admin', () => {
  const api = () => getApiContainer().admin;

  const currentCards = ref<CardListState | null>(null);
  const bunkerCards = ref<BunkerCardDto[]>([]);
  const packs = ref<CardPackDto[]>([]);
  const presets = ref<PersonalityPresetDto[]>([]);

  const dashboard = ref<DashboardStats | null>(null);
  const loading = ref(false);
  const dashboardLoading = ref(false);

  // --- Cards ---

  async function loadCards(type: CardTypeKey, skip = 0, take = PAGE_SIZE): Promise<void> {
    loading.value = true;
    try {
      const page = await api().listCards(type, skip, take);
      currentCards.value = { type, total: page.total, cards: page.cards, skip, take };
    } finally {
      loading.value = false;
    }
  }

  async function createCard(type: CardTypeKey, body: CreateCardRequest): Promise<void> {
    await api().createCard(type, body);
    await loadCards(type, currentCards.value?.skip ?? 0, currentCards.value?.take ?? PAGE_SIZE);
  }

  async function updateCard(type: CardTypeKey, id: string, body: CreateCardRequest): Promise<void> {
    await api().updateCard(type, id, body);
    await loadCards(type, currentCards.value?.skip ?? 0, currentCards.value?.take ?? PAGE_SIZE);
  }

  async function deleteCard(type: CardTypeKey, id: string): Promise<void> {
    await api().deleteCard(id);
    await loadCards(type, currentCards.value?.skip ?? 0, currentCards.value?.take ?? PAGE_SIZE);
  }

  // --- Bunker cards ---

  async function loadBunkerCards(): Promise<void> {
    loading.value = true;
    try {
      bunkerCards.value = await api().listBunkerCards();
    } finally {
      loading.value = false;
    }
  }

  async function createBunkerCard(body: CreateBunkerCardRequest): Promise<void> {
    await api().createBunkerCard(body);
    await loadBunkerCards();
  }

  async function updateBunkerCard(id: string, body: CreateBunkerCardRequest): Promise<void> {
    await api().updateBunkerCard(id, body);
    await loadBunkerCards();
  }

  async function deleteBunkerCard(id: string): Promise<void> {
    await api().deleteBunkerCard(id);
    await loadBunkerCards();
  }

  // --- Card packs ---

  async function loadPacks(): Promise<void> {
    loading.value = true;
    try {
      packs.value = await api().listPacksFull();
    } finally {
      loading.value = false;
    }
  }

  async function createPack(body: CreateCardPackRequest): Promise<void> {
    await api().createPack(body);
    await loadPacks();
  }

  async function updatePack(id: string, body: CreateCardPackRequest): Promise<void> {
    await api().updatePack(id, body);
    await loadPacks();
  }

  async function deletePack(id: string): Promise<void> {
    await api().deletePack(id);
    await loadPacks();
  }

  // --- Personality presets ---

  async function loadPresets(): Promise<void> {
    loading.value = true;
    try {
      presets.value = await api().listPresets();
    } finally {
      loading.value = false;
    }
  }

  async function createPreset(body: CreatePersonalityPresetRequest): Promise<void> {
    await api().createPreset(body);
    await loadPresets();
  }

  async function updatePreset(id: string, body: CreatePersonalityPresetRequest): Promise<void> {
    await api().updatePreset(id, body);
    await loadPresets();
  }

  async function deletePreset(id: string): Promise<void> {
    await api().deletePreset(id);
    await loadPresets();
  }

  // --- Dashboard ---

  // Stats are derived from the existing list endpoints: card counts come from each type's
  // paginated `total` (fetched with take=1 so we don't pull every card), the rest from the
  // unpaginated list lengths.
  async function loadDashboard(): Promise<void> {
    dashboardLoading.value = true;
    try {
      const cardResults = await Promise.all(
        CARD_TYPES.map(async (t) => {
          const page = await api().listCards(t.key, 0, 1);
          return [t.key, page.total] as const;
        }),
      );
      const cardCounts: Record<string, number> = {};
      for (const [key, total] of cardResults) cardCounts[key] = total;

      const [bunker, allPacks, allPresets] = await Promise.all([
        api().listBunkerCards(),
        api().listPacksFull(),
        api().listPresets(),
      ]);

      dashboard.value = {
        cardCounts,
        bunkerCount: bunker.length,
        packCount: allPacks.length,
        presetCount: allPresets.length,
      };
    } finally {
      dashboardLoading.value = false;
    }
  }

  return {
    currentCards,
    bunkerCards,
    packs,
    presets,
    dashboard,
    loading,
    dashboardLoading,
    pageSize: PAGE_SIZE,
    loadCards,
    createCard,
    updateCard,
    deleteCard,
    loadBunkerCards,
    createBunkerCard,
    updateBunkerCard,
    deleteBunkerCard,
    loadPacks,
    createPack,
    updatePack,
    deletePack,
    loadPresets,
    createPreset,
    updatePreset,
    deletePreset,
    loadDashboard,
  };
});