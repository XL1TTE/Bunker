<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useAdminStore } from '@/stores/admin.store';
import { useToast } from '@/composables/useToast';
import type { PersonalityPresetDto, CreatePersonalityPresetRequest } from '@/types/admin.types';
import ResourceTable, { type ResourceColumn } from '@/components/admin/ResourceTable.vue';
import Modal from '@/components/common/Modal.vue';
import ConfirmDialog from '@/components/common/ConfirmDialog.vue';
import styles from '@/views/page.module.css';

const admin = useAdminStore();
const { run, success } = useToast();

onMounted(async () => {
  await run(() => admin.loadPresets(), { message: 'Could not load presets.' });
});

const modalOpen = ref(false);
const editing = ref<PersonalityPresetDto | null>(null);
const title = ref('');
const description = ref('');
const inputError = ref<string | null>(null);
const saving = ref(false);

const deleteTarget = ref<PersonalityPresetDto | null>(null);
const deleting = ref(false);

const columns: ResourceColumn<PersonalityPresetDto>[] = [
  { label: 'Title', value: (r) => r.title },
  { label: 'Description', value: (r) => r.description || '—' },
];

function openAdd(): void {
  editing.value = null;
  title.value = '';
  description.value = '';
  inputError.value = null;
  modalOpen.value = true;
}

function openEdit(preset: PersonalityPresetDto): void {
  editing.value = preset;
  title.value = preset.title;
  description.value = preset.description;
  inputError.value = null;
  modalOpen.value = true;
}

function closeModal(): void {
  modalOpen.value = false;
}

function validate(): CreatePersonalityPresetRequest | null {
  if (title.value.trim().length === 0) {
    inputError.value = 'Give the preset a title.';
    return null;
  }
  if (description.value.trim().length === 0) {
    inputError.value = 'Add a short description of the personality.';
    return null;
  }
  inputError.value = null;
  return { title: title.value.trim(), description: description.value.trim() };
}

async function save(): Promise<void> {
  const body = validate();
  if (!body) return;
  saving.value = true;
  try {
    const preset = editing.value;
    const ok = await run(() =>
      preset ? admin.updatePreset(preset.id, body) : admin.createPreset(body),
    );
    if (ok === null) return;
    success(preset ? 'Preset updated.' : 'Preset added.');
    modalOpen.value = false;
  } finally {
    saving.value = false;
  }
}

function askDelete(preset: PersonalityPresetDto): void {
  deleteTarget.value = preset;
}
function cancelDelete(): void {
  deleteTarget.value = null;
}
async function confirmDelete(): Promise<void> {
  const target = deleteTarget.value;
  if (!target) return;
  deleting.value = true;
  try {
    const ok = await run(() => admin.deletePreset(target.id));
    if (ok === null) return;
    success('Preset deleted.');
    deleteTarget.value = null;
  } finally {
    deleting.value = false;
  }
}

const count = computed(() => admin.presets.length);
</script>

<template>
  <section :class="styles.page">
    <div :class="styles.heading">
      <div :class="styles.headingText">
        <h1 :class="styles.title">Bot presets</h1>
        <p :class="styles.subtitle">{{ count }} personalit{{ count === 1 ? 'y' : 'ies' }} for AI players</p>
      </div>
      <div :class="styles.toolbar">
        <button class="btn btnPrimary" @click="openAdd">+ Add preset</button>
      </div>
    </div>

    <ResourceTable
      :columns="columns"
      :rows="admin.presets"
      :row-key="(p: PersonalityPresetDto) => p.id"
      :loading="admin.loading && admin.presets.length === 0"
      empty-text="No presets yet. Add a personality for the AI bots to play as."
      @edit="openEdit"
      @delete="askDelete"
    />

    <Modal :open="modalOpen" :title="editing ? 'Edit preset' : 'Add preset'" @close="closeModal">
      <form @submit.prevent="save">
        <div class="field">
          <label class="label" for="preset-title">Title</label>
          <input id="preset-title" v-model="title" type="text" placeholder="e.g. Cautious survivor" autofocus />
        </div>
        <div class="field">
          <label class="label" for="preset-desc">Description</label>
          <textarea id="preset-desc" v-model="description" rows="3" placeholder="How this bot thinks and behaves" />
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
      title="Delete this preset?"
      :message="`“${deleteTarget?.title ?? ''}” will no longer be available for AI players.`"
      confirm-label="Delete"
      cancel-label="Keep it"
      destructive
      :busy="deleting"
      @confirm="confirmDelete"
      @close="cancelDelete"
    />
  </section>
</template>