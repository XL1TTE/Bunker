<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { RouterLink } from 'vue-router';
import { useAdminStore } from '@/stores/admin.store';
import { useToast } from '@/composables/useToast';
import { findCardType, type UnknownCard, type CreateCardRequest } from '@/types/admin.types';
import ResourceTable, { type ResourceColumn } from '@/components/admin/ResourceTable.vue';
import Modal from '@/components/common/Modal.vue';
import ConfirmDialog from '@/components/common/ConfirmDialog.vue';
import styles from '@/views/page.module.css';

const props = defineProps<{ type: string }>();

const admin = useAdminStore();
const { run, success } = useToast();

const config = computed(() => findCardType(props.type));
const list = computed(() => (admin.currentCards?.type === props.type ? admin.currentCards : null));

const modalOpen = ref(false);
const editingCard = ref<UnknownCard | null>(null);
const inputValue = ref('');
const inputError = ref<string | null>(null);
const saving = ref(false);

const deleteTarget = ref<UnknownCard | null>(null);
const deleting = ref(false);

const isInt = computed(() => config.value?.valueType === 'int');

async function reload(): Promise<void> {
  if (!config.value) return;
  await run(() => admin.loadCards(config.value!.key), { message: 'Could not load cards.' });
}

// Reload whenever the route's :type changes (sidebar navigation between card types).
watch(
  () => props.type,
  () => void reload(),
  { immediate: true },
);

function openAdd(): void {
  editingCard.value = null;
  inputValue.value = '';
  inputError.value = null;
  modalOpen.value = true;
}

function openEdit(card: UnknownCard): void {
  if (!config.value) return;
  editingCard.value = card;
  inputValue.value = String(card[config.value.fieldName] ?? '');
  inputError.value = null;
  modalOpen.value = true;
}

function closeModal(): void {
  modalOpen.value = false;
}

// Validate the single field. Age is an integer 0–254; strings just need to be non-empty.
function validate(): CreateCardRequest | null {
  const cfg = config.value;
  if (!cfg) return null;
  const raw = inputValue.value.trim();
  if (raw.length === 0) {
    inputError.value = 'Enter a value.';
    return null;
  }
  if (isInt.value) {
    const n = Number(raw);
    if (!Number.isInteger(n) || n < 0 || n > 254) {
      inputError.value = 'Age must be a whole number from 0 to 254.';
      return null;
    }
    return { [cfg.fieldName]: n };
  }
  return { [cfg.fieldName]: raw };
}

async function save(): Promise<void> {
  const cfg = config.value;
  if (!cfg) return;
  const body = validate();
  if (!body) return;
  inputError.value = null;
  saving.value = true;
  try {
    const editing = editingCard.value;
    const ok = await run(() =>
      editing ? admin.updateCard(cfg.key, editing.id, body) : admin.createCard(cfg.key, body),
    );
    if (ok === null) return; // error already toasted
    success(editing ? 'Card updated.' : 'Card added.');
    modalOpen.value = false;
  } finally {
    saving.value = false;
  }
}

function askDelete(card: UnknownCard): void {
  deleteTarget.value = card;
}

function cancelDelete(): void {
  deleteTarget.value = null;
}

async function confirmDelete(): Promise<void> {
  const cfg = config.value;
  const target = deleteTarget.value;
  if (!cfg || !target) return;
  deleting.value = true;
  try {
    const ok = await run(() => admin.deleteCard(cfg.key, target.id));
    if (ok === null) return;
    success('Card deleted.');
    deleteTarget.value = null;
  } finally {
    deleting.value = false;
  }
}

const columns = computed<ResourceColumn<UnknownCard>[]>(() => {
  const cfg = config.value;
  if (!cfg) return [];
  return [{ label: cfg.label, value: (row) => String(row[cfg.fieldName] ?? '') }];
});

// Paging — the store keeps skip/take in currentCards.
const pageSize = admin.pageSize;
function goPrev(): void {
  if (!list.value || list.value.skip === 0) return;
  void run(() => admin.loadCards(config.value!.key, list.value!.skip - pageSize));
}
function goNext(): void {
  if (!list.value) return;
  if (list.value.skip + list.value.cards.length >= list.value.total) return;
  void run(() => admin.loadCards(config.value!.key, list.value!.skip + pageSize));
}
const rangeStart = computed(() => (list.value ? list.value.skip + 1 : 0));
const rangeEnd = computed(() =>
  list.value ? list.value.skip + list.value.cards.length : 0,
);
</script>

<template>
  <section v-if="!config" :class="styles.page">
    <div :class="styles.heading">
      <div :class="styles.headingText">
        <h1 :class="styles.title">Unknown card type</h1>
        <p :class="styles.subtitle">“{{ type }}” isn't one of the character-card types.</p>
      </div>
    </div>
    <RouterLink to="/">← Back to dashboard</RouterLink>
  </section>

  <section v-else :class="styles.page">
    <div :class="styles.heading">
      <div :class="styles.headingText">
        <h1 :class="styles.title">{{ config.label }} cards</h1>
        <p :class="styles.subtitle">{{ list ? `${list.total} in the deck` : 'Loading…' }}</p>
      </div>
      <div :class="styles.toolbar">
        <button class="btn btnPrimary" @click="openAdd">+ Add {{ config.label.toLowerCase() }}</button>
      </div>
    </div>

    <ResourceTable
      :columns="columns"
      :rows="list?.cards ?? []"
      :row-key="(c: UnknownCard) => c.id"
      :loading="admin.loading && !list"
      :empty-text="`No ${config.label.toLowerCase()} cards yet. Add one to start building the deck.`"
      @edit="openEdit"
      @delete="askDelete"
    />

    <div v-if="list && list.total > list.cards.length" :class="styles.pager">
      <span :class="styles.pagerInfo">{{ rangeStart }}–{{ rangeEnd }} of {{ list.total }}</span>
      <div :class="styles.pagerButtons">
        <button class="btn btnGhost btnSm" :disabled="list.skip === 0" @click="goPrev">Prev</button>
        <button
          class="btn btnGhost btnSm"
          :disabled="list.skip + list.cards.length >= list.total"
          @click="goNext"
        >
          Next
        </button>
      </div>
    </div>

    <Modal :open="modalOpen" :title="editingCard ? `Edit ${config.label.toLowerCase()} card` : `Add ${config.label.toLowerCase()} card`" @close="closeModal">
      <form @submit.prevent="save">
        <div class="field">
          <label class="label" :for="`card-${config.key}`">{{ config.label }}</label>
          <input
            :id="`card-${config.key}`"
            v-model="inputValue"
            :type="isInt ? 'number' : 'text'"
            :placeholder="config.placeholder"
            :min="isInt ? 0 : undefined"
            :max="isInt ? 254 : undefined"
            autofocus
          />
          <span v-if="inputError" class="fieldError">{{ inputError }}</span>
        </div>
        <div class="formActions">
          <button type="button" class="btn btnGhost" :disabled="saving" @click="closeModal">Cancel</button>
          <button type="submit" class="btn btnPrimary" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
        </div>
      </form>
    </Modal>

    <ConfirmDialog
      :open="deleteTarget !== null"
      title="Delete this card?"
      :message="`This removes the ${config.label.toLowerCase()} card from the deck. Lobbies already using it keep their copy.`"
      confirm-label="Delete"
      cancel-label="Keep it"
      destructive
      :busy="deleting"
      @confirm="confirmDelete"
      @close="cancelDelete"
    />
  </section>
</template>