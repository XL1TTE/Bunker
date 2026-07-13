<script setup lang="ts">
import styles from '@/components/game/roulette-spinner.module.css';

// Cosmetic tie-break indicator. `compact` renders it inline so it can sit
// inside the action bar; the default (stacked) layout is kept for any future
// standalone use.
withDefaults(
  defineProps<{
    tiedParticipantIds: string[];
    nameOf: (participantId: string) => string;
    compact?: boolean;
  }>(),
  { compact: false },
);
</script>

<template>
  <div :class="[styles.wrap, compact ? styles.compact : '']">
    <div :class="styles.head">
      <span :class="styles.spinner" aria-hidden="true"></span>
      <span :class="styles.eyebrow">Roulette · Tie-break</span>
    </div>
    <p :class="styles.text">Tied players are being decided at random…</p>
    <ul :class="styles.tiedList">
      <li v-for="(id, i) in tiedParticipantIds" :key="id" :class="styles.tiedItem">
        <span :class="styles.tiedIndex">{{ i + 1 }}</span>
        <span :class="styles.tiedName">{{ nameOf(id) }}</span>
      </li>
    </ul>
  </div>
</template>