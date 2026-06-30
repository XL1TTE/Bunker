import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { getApiContainer } from '@/api/register';
import type { CardPackPreview, PersonalityPresetPreview } from '@/types/content.types';

export const useCatalogStore = defineStore('catalog', () => {
  const packs = ref<CardPackPreview[]>([]);
  const presets = ref<PersonalityPresetPreview[]>([]);
  const loaded = ref(false);
  const loading = ref(false);

  async function load(): Promise<void> {
    if (loaded.value || loading.value) return;
    loading.value = true;
    try {
      const api = getApiContainer().content;
      const [packsData, presetsData] = await Promise.all([
        api.listCardPacks(),
        api.listPersonalityPresets(),
      ]);
      packs.value = packsData;
      presets.value = presetsData;
      loaded.value = true;
    } finally {
      loading.value = false;
    }
  }

  const packById = computed(() => {
    const map = new Map<string, CardPackPreview>();
    for (const p of packs.value) map.set(p.id, p);
    return map;
  });

  const presetById = computed(() => {
    const map = new Map<string, PersonalityPresetPreview>();
    for (const p of presets.value) map.set(p.id, p);
    return map;
  });

  return { packs, presets, loaded, loading, load, packById, presetById };
});