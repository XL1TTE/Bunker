<script setup lang="ts">
import Modal from '@/components/common/Modal.vue';
import styles from '@/components/common/confirm-dialog.module.css';

withDefaults(
  defineProps<{
    open: boolean;
    title: string;
    message: string;
    confirmLabel?: string;
    cancelLabel?: string;
    destructive?: boolean;
    busy?: boolean;
  }>(),
  {
    confirmLabel: 'Confirm',
    cancelLabel: 'Cancel',
    destructive: false,
    busy: false,
  },
);

defineEmits<{ confirm: []; close: [] }>();
</script>

<template>
  <Modal :open="open" :title="title" @close="$emit('close')">
    <p :class="styles.message">{{ message }}</p>
    <div :class="styles.actions">
      <button type="button" :class="styles.cancel" :disabled="busy" @click="$emit('close')">
        {{ cancelLabel }}
      </button>
      <button
        type="button"
        :class="[styles.confirm, destructive && styles.confirmDanger]"
        :disabled="busy"
        @click="$emit('confirm')"
      >
        {{ busy ? 'Working…' : confirmLabel }}
      </button>
    </div>
  </Modal>
</template>