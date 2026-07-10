<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useAdminStore } from '@/stores/admin.store';
import { useToast } from '@/composables/useToast';
import type { CardPackDto, CreateCardPackRequest } from '@/types/admin.types';
import ResourceTable, { type ResourceColumn } from '@/components/admin/ResourceTable.vue';
import PackEditor from '@/components/admin/PackEditor.vue';
import ConfirmDialog from '@/components/common/ConfirmDialog.vue';
import styles from '@/views/page.module.css';

const admin = useAdminStore();
const { run, success } = useToast();

onMounted(async () => {
  await run(() => admin.loadPacks(), { message: 'Could not load packs.' });
});

const editorOpen = ref(false);
const editingPack = ref<CardPackDto | null>(null);
const saving = ref(false);
const editor = ref<InstanceType<typeof PackEditor> | null>(null);

const deleteTarget = ref<CardPackDto | null>(null);
const deleting = ref(false);

const columns: ResourceColumn<CardPackDto>[] = [
  { label: 'Title', value: (r) => r.title },
  { label: 'Description', value: (r) => r.description || '—' },
  { label: 'Cards', value: (r) => String(r.cardIds.length), width: '80px' },
];

function openAdd(): void {
  editingPack.value = null;
  editorOpen.value = true;
}

function openEdit(pack: CardPackDto): void {
  editingPack.value = pack;
  editorOpen.value = true;
}

function closeEditor(): void {
  editorOpen.value = false;
}

async function handleSave({ id, body }: { id: string | null; body: CreateCardPackRequest }): Promise<void> {
  saving.value = true;
  try {
    const ok = await run(() => (id ? admin.updatePack(id, body) : admin.createPack(body)));
    if (ok === null) {
      // Surface the failure inline in the editor (run() already toasted it).
      editor.value?.reportSaveError(new Error('Save failed — see the toast for details.'));
      return;
    }
    success(id ? 'Pack updated.' : 'Pack created.');
    editorOpen.value = false;
  } finally {
    saving.value = false;
  }
}

function askDelete(pack: CardPackDto): void {
  deleteTarget.value = pack;
}
function cancelDelete(): void {
  deleteTarget.value = null;
}
async function confirmDelete(): Promise<void> {
  const target = deleteTarget.value;
  if (!target) return;
  deleting.value = true;
  try {
    const ok = await run(() => admin.deletePack(target.id));
    if (ok === null) return;
    success('Pack deleted.');
    deleteTarget.value = null;
  } finally {
    deleting.value = false;
  }
}

const count = computed(() => admin.packs.length);
</script>

<template>
  <section :class="styles.page">
    <div :class="styles.heading">
      <div :class="styles.headingText">
        <h1 :class="styles.title">Card packs</h1>
        <p :class="styles.subtitle">{{ count }} pack{{ count === 1 ? '' : 's' }} available to lobbies</p>
      </div>
      <div :class="styles.toolbar">
        <button class="btn btnPrimary" @click="openAdd">+ Add pack</button>
      </div>
    </div>

    <ResourceTable
      :columns="columns"
      :rows="admin.packs"
      :row-key="(p: CardPackDto) => p.id"
      :loading="admin.loading && admin.packs.length === 0"
      empty-text="No packs yet. Add one and pick the cards it should deal from."
      @edit="openEdit"
      @delete="askDelete"
    />

    <PackEditor
      ref="editor"
      :open="editorOpen"
      :pack="editingPack"
      :saving="saving"
      @close="closeEditor"
      @save="handleSave"
    />

    <ConfirmDialog
      :open="deleteTarget !== null"
      title="Delete this pack?"
      :message="`“${deleteTarget?.title ?? ''}” will be removed. Lobbies already using it keep their copy.`"
      confirm-label="Delete"
      cancel-label="Keep it"
      destructive
      :busy="deleting"
      @confirm="confirmDelete"
      @close="cancelDelete"
    />
  </section>
</template>