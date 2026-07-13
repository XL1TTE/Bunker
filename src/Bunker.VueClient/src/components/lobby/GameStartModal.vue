<script setup lang="ts">
import { computed } from 'vue';
import { useLobbyStore } from '@/stores/lobby.store';
import Modal from '@/components/common/Modal.vue';
import styles from '@/components/lobby/game-start-modal.module.css';

const lobbyStore = useLobbyStore();

const steps = computed(() => lobbyStore.startSteps);
const failed = computed(() => lobbyStore.startFailed);
const failReason = computed(() => lobbyStore.startFailReason);

function onCloseAttempt(): void {
  if (failed.value) lobbyStore.dismissStart();
}
function dismiss(): void {
  lobbyStore.dismissStart();
}
</script>

<template>
  <Modal :open="lobbyStore.startInProgress" title="Starting game" @close="onCloseAttempt">
    <ol :class="styles.steps">
      <li
        v-for="s in steps"
        :key="s.id"
        :class="[
          styles.step,
          s.status === 'succeeded' && styles.stepSucceeded,
          s.status === 'failed' && styles.stepFailed,
          s.status === 'started' && styles.stepStarted,
          s.status === 'pending' && styles.stepPending,
        ]"
      >
        <span :class="styles.indicator" aria-hidden="true">
          <template v-if="s.status === 'succeeded'">✓</template>
          <template v-else-if="s.status === 'failed'">!</template>
          <template v-else-if="s.status === 'started'"><span :class="styles.spinner" /></template>
          <template v-else>○</template>
        </span>
        <div :class="styles.stepBody">
          <span :class="styles.stepLabel">{{ s.label }}</span>
          <span v-if="s.status === 'failed' && s.message" :class="styles.stepReason">{{ s.message }}</span>
        </div>
      </li>
    </ol>
    <div v-if="failed" :class="styles.actions">
      <p v-if="failReason" :class="styles.failSummary">{{ failReason }}</p>
      <button :class="styles.dismissButton" @click="dismiss">Dismiss</button>
    </div>
  </Modal>
</template>