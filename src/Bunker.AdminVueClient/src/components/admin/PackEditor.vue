<script setup lang="ts">
import { ref, watch } from 'vue';
import { getApiContainer } from '@/api/register';
import Modal from '@/components/common/Modal.vue';
import { CARD_TYPES, type CardTypeKey, type CardPackDto, type CreateCardPackRequest, type UnknownCard } from '@/types/admin.types';
import { extractErrorMessage } from '@/utils/errorMessage';
import styles from '@/components/admin/pack-editor.module.css';

const props = defineProps<{
  open: boolean;
  // The pack being edited, or null for create mode.
  pack: CardPackDto | null;
  // True while the parent is persisting — disables the save button.
  saving?: boolean;
}>();

const emit = defineEmits<{
  close: [];
  // Parent owns the store call + toast so success/error stay in one place.
  save: [payload: { id: string | null; body: CreateCardPackRequest }];
}>();

const title = ref('');
const description = ref('');
const generationPrompt = ref('');
const selectedIds = ref<Set<string>>(new Set());
const cardsByType = ref<Record<CardTypeKey, UnknownCard[]>>({} as Record<CardTypeKey, UnknownCard[]>);
const loadingCards = ref(false);
const saveError = ref<string | null>(null);

const selectedCount = ref(0);

// Pull every card of every type once per open so the picker can offer the full deck.
// take=1000 is deliberately large — the admin deck is small; if it ever grows past
// this, paginate the picker.
async function loadAllCards(): Promise<void> {
  loadingCards.value = true;
  try {
    const api = getApiContainer().admin;
    const entries = await Promise.all(
      CARD_TYPES.map(async (t) => {
        const page = await api.listCards(t.key, 0, 1000);
        return [t.key, page.cards] as const;
      }),
    );
    const map = {} as Record<CardTypeKey, UnknownCard[]>;
    for (const [key, cards] of entries) map[key] = cards;
    cardsByType.value = map;
  } finally {
    loadingCards.value = false;
  }
}

function resetForm(): void {
  const p = props.pack;
  title.value = p?.title ?? '';
  description.value = p?.description ?? '';
  generationPrompt.value = p?.generationPrompt ?? '';
  selectedIds.value = new Set(p?.cardIds ?? []);
  selectedCount.value = selectedIds.value.size;
  saveError.value = null;
}

watch(
  () => props.open,
  (open) => {
    if (!open) return;
    resetForm();
    void loadAllCards();
  },
);

function cardLabel(card: UnknownCard, fieldName: string): string {
  const v = card[fieldName];
  return v === undefined || v === null ? '(empty)' : String(v);
}

function toggle(cardId: string): void {
  const next = new Set(selectedIds.value);
  if (next.has(cardId)) next.delete(cardId);
  else next.add(cardId);
  selectedIds.value = next;
  selectedCount.value = next.size;
}

function countSelectedForType(type: CardTypeKey): number {
  const ids = selectedIds.value;
  return cardsByType.value[type]?.filter((c) => ids.has(c.id)).length ?? 0;
}

const canSave = () => title.value.trim().length > 0 && !loadingCards.value && !props.saving;

function submit(): void {
  if (!canSave()) return;
  saveError.value = null;
  emit('save', {
    id: props.pack?.id ?? null,
    body: {
      title: title.value.trim(),
      description: description.value.trim(),
      generationPrompt: generationPrompt.value.trim(),
      cardIds: [...selectedIds.value],
    },
  });
}

// Parent reports a save failure back so we can show it inline rather than only as a toast.
defineExpose({
  reportSaveError(err: unknown) {
    saveError.value = extractErrorMessage(err);
  },
});
</script>

<template>
  <Modal :open="open" :title="pack ? 'Edit pack' : 'New pack'" @close="$emit('close')">
    <form @submit.prevent="submit">
      <div class="field">
        <label class="label" for="pack-title">Title</label>
        <input id="pack-title" v-model="title" type="text" placeholder="e.g. Classic survival deck" autofocus />
      </div>

      <div class="field">
        <label class="label" for="pack-desc">Description</label>
        <textarea id="pack-desc" v-model="description" rows="2" placeholder="What this pack is for" />
      </div>

      <div class="field">
        <label class="label" for="pack-prompt">Generation prompt</label>
        <textarea id="pack-prompt" v-model="generationPrompt" rows="2" placeholder="Optional — passed to the AI when generating cards" />
        <span class="helpText">Used by the AI service when it tops up this pack.</span>
      </div>

      <div :class="styles.picker">
        <div :class="styles.pickerHead">
          <span class="label">Cards in this pack</span>
          <span :class="styles.pickerCount">{{ selectedCount }} selected</span>
        </div>

        <p v-if="loadingCards" :class="styles.pickerLoading">Loading cards…</p>

        <div v-else :class="styles.groups">
          <section v-for="t in CARD_TYPES" :key="t.key" :class="styles.group">
            <div :class="styles.groupHead">
              <span :class="styles.groupLabel">{{ t.label }}</span>
              <span :class="styles.groupCount">{{ countSelectedForType(t.key) }} / {{ cardsByType[t.key]?.length ?? 0 }}</span>
            </div>
            <ul v-if="cardsByType[t.key]?.length" :class="styles.cardList">
              <li v-for="card in cardsByType[t.key]" :key="card.id">
                <label :class="styles.cardOption">
                  <input type="checkbox" :checked="selectedIds.has(card.id)" @change="toggle(card.id)" />
                  <span :class="styles.cardText">{{ cardLabel(card, t.fieldName) }}</span>
                </label>
              </li>
            </ul>
            <p v-else :class="styles.groupEmpty">No {{ t.label.toLowerCase() }} cards yet.</p>
          </section>
        </div>
      </div>

      <p v-if="saveError" :class="styles.saveError">{{ saveError }}</p>

      <div class="formActions">
        <button type="button" class="btn btnGhost" :disabled="props.saving" @click="$emit('close')">Cancel</button>
        <button type="submit" class="btn btnPrimary" :disabled="!canSave()">{{ props.saving ? 'Saving…' : 'Save pack' }}</button>
      </div>
    </form>
  </Modal>
</template>