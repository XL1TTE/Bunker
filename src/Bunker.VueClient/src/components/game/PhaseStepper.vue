<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/game.store';
import { GAME_PHASE_ORDER, PHASE_LABELS } from '@/components/game/game-phases';
import styles from '@/components/game/phase-stepper.module.css';

const gameStore = useGameStore();
const currentPhase = computed(() => gameStore.phase);
</script>

<template>
  <ol :class="styles.stepper" aria-label="Game phases">
    <li
      v-for="phase in GAME_PHASE_ORDER"
      :key="phase"
      :class="[styles.chip, phase === currentPhase ? styles.chipCurrent : '']"
      :aria-current="phase === currentPhase ? 'step' : undefined"
    >
      <span :class="styles.dot" aria-hidden="true"></span>
      <span :class="styles.label">{{ PHASE_LABELS[phase] }}</span>
    </li>
  </ol>
</template>