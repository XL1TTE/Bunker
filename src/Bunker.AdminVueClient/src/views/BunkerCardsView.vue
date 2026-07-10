<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useAdminStore } from '@/stores/admin.store';
import { useToast } from '@/composables/useToast';
import type { BunkerCardDto, CreateBunkerCardRequest } from '@/types/admin.types';
import ResourceTable, { type ResourceColumn } from '@/components/admin/ResourceTable.vue';
import Modal from '@/components/common/Modal.vue';
import ConfirmDialog from '@/components/common/ConfirmDialog.vue';
import styles from '@/views/page.module.css';

const admin = useAdminStore();
const { run, success } = useToast();

onMounted(async () => {
  await run(() => admin.loadBunkerCards(), { message: 'Could not load bunker cards.' });
});

const modalOpen = ref(false);
const editing = ref<BunkerCardDto | null>(null);
const catastrophe = ref('');
const survivalDuration = ref('');
const bunkerEnvironment = ref('');
const inputError = ref<string | null>(null);
const saving = ref(false);

const deleteTarget = ref<BunkerCardDto | null>(null);
const deleting = ref(false);

const columns: ResourceColumn<BunkerCardDto>[] = [
  { label: 'Catastrophe', value: (r) => r.catastrophe },
  { label: 'Survival', value: (r) => r.survivalDuration, width: '160px' },
  { label: 'Bunker environment', value: (r) => r.bunkerEnvironment },
];

function openAdd(): void {
  editing.value = null;
  catastrophe.value = '';
  survivalDuration.value = '';
  bunkerEnvironment.value = '';
  inputError.value = null;
  modalOpen.value = true;
}

function openEdit(card: BunkerCardDto): void {
  editing.value = card;
  catastrophe.value = card.catastrophe;
  survivalDuration.value = card.survivalDuration;
  bunkerEnvironment.value = card.bunkerEnvironment;
  inputError.value = null;
  modalOpen.value = true;
}

function closeModal(): void {
  modalOpen.value = false;
}

// Min lengths match the backend validator — caught here first for a snappier message.
function validate(): CreateBunkerCardRequest | null {
  if (catastrophe.value.trim().length < 8) {
    inputError.value = 'Catastrophe needs at least 8 characters.';
    return null;
  }
  if (survivalDuration.value.trim().length < 3) {
    inputError.value = 'Survival duration needs at least 3 characters.';
    return null;
  }
  if (bunkerEnvironment.value.trim().length < 10) {
    inputError.value = 'Bunker environment needs at least 10 characters.';
    return null;
  }
  inputError.value = null;
  return {
    catastrophe: catastrophe.value.trim(),
    survivalDuration: survivalDuration.value.trim(),
    bunkerEnvironment: bunkerEnvironment.value.trim(),
  };
}

async function save(): Promise<void> {
  const body = validate();
  if (!body) return;
  saving.value = true;
  try {
    const card = editing.value;
    const ok = await run(() =>
      card ? admin.updateBunkerCard(card.id, body) : admin.createBunkerCard(body),
    );
    if (ok === null) return;
    success(card ? 'Bunker card updated.' : 'Bunker card added.');
    modalOpen.value = false;
  } finally {
    saving.value = false;
  }
}

function askDelete(card: BunkerCardDto): void {
  deleteTarget.value = card;
}
function cancelDelete(): void {
  deleteTarget.value = null;
}
async function confirmDelete(): Promise<void> {
  const target = deleteTarget.value;
  if (!target) return;
  deleting.value = true;
  try {
    const ok = await run(() => admin.deleteBunkerCard(target.id));
    if (ok === null) return;
    success('Bunker card deleted.');
    deleteTarget.value = null;
  } finally {
    deleting.value = false;
  }
}

const count = computed(() => admin.bunkerCards.length);
</script>

<template>
  <section :class="styles.page">
    <div :class="styles.heading">
      <div :class="styles.headingText">
        <h1 :class="styles.title">Bunker cards</h1>
        <p :class="styles.subtitle">{{ count }} scenario{{ count === 1 ? '' : 's' }} in the deck</p>
      </div>
      <div :class="styles.toolbar">
        <button class="btn btnPrimary" @click="openAdd">+ Add bunker card</button>
      </div>
    </div>

    <ResourceTable
      :columns="columns"
      :rows="admin.bunkerCards"
      :row-key="(c: BunkerCardDto) => c.id"
      :loading="admin.loading && admin.bunkerCards.length === 0"
      empty-text="No bunker cards yet. Add one so games have a scenario to survive."
      @edit="openEdit"
      @delete="askDelete"
    />

    <Modal :open="modalOpen" :title="editing ? 'Edit bunker card' : 'Add bunker card'" @close="closeModal">
      <form @submit.prevent="save">
        <div class="field">
          <label class="label" for="bunker-catastrophe">Catastrophe</label>
          <input id="bunker-catastrophe" v-model="catastrophe" type="text" placeholder="e.g. Global nuclear fallout" autofocus />
        </div>
        <div class="field">
          <label class="label" for="bunker-survival">Survival duration</label>
          <input id="bunker-survival" v-model="survivalDuration" type="text" placeholder="e.g. 12 months" />
        </div>
        <div class="field">
          <label class="label" for="bunker-env">Bunker environment</label>
          <textarea id="bunker-env" v-model="bunkerEnvironment" rows="3" placeholder="Describe the conditions inside the bunker" />
        </div>
        <p v-if="inputError" class="fieldError">{{ inputError }}</p>
        <div class="formActions">
          <button type="button" class="btn btnGhost" :disabled="saving" @click="closeModal">Cancel</button>
          <button type="submit" class="btn btnPrimary" :disabled="saving">{{ saving ? 'Saving…' : 'Save' }}</button>
        </div>
      </form>
    </Modal>

    <ConfirmDialog
      :open="deleteTarget !== null"
      title="Delete this bunker card?"
      message="This removes the scenario from the deck. Games in progress keep their copy."
      confirm-label="Delete"
      cancel-label="Keep it"
      destructive
      :busy="deleting"
      @confirm="confirmDelete"
      @close="cancelDelete"
    />
  </section>
</template>