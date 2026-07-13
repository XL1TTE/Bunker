<script setup lang="ts">
import { onBeforeUnmount, ref, watch } from 'vue';
import { useGameStore } from '@/stores/game.store';
import { MODAL_PHASES, PHASE_DESCRIPTIONS, PHASE_LABELS } from '@/components/game/game-phases';
import Modal from '@/components/common/Modal.vue';
import styles from '@/components/game/phase-change-modal.module.css';

const gameStore = useGameStore();
const open = ref(false);
const phaseLabel = ref('');
const phaseDescription = ref('');
let dismissTimer: number | null = null;

function clearTimer(): void {
  if (dismissTimer !== null) {
    window.clearTimeout(dismissTimer);
    dismissTimer = null;
  }
}

function dismiss(): void {
  clearTimer();
  open.value = false;
}

watch(
  () => gameStore.phase,
  (phase, oldPhase) => {
    if (!phase || !MODAL_PHASES.has(phase) || oldPhase == null) {
      clearTimer();
      open.value = false;
      return;
    }
    phaseLabel.value = PHASE_LABELS[phase];
    phaseDescription.value = PHASE_DESCRIPTIONS[phase];
    open.value = true;
    clearTimer();
    dismissTimer = window.setTimeout(() => {
      open.value = false;
      dismissTimer = null;
    }, 3500);
  },
);

onBeforeUnmount(() => {
  clearTimer();
});
</script>

<template>
  <Modal :open="open" :title="phaseLabel" @close="dismiss">
    <p :class="styles.description">{{ phaseDescription }}</p>
  </Modal>
</template>