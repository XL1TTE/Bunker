<script setup lang="ts">
import type { TallyDto } from '@/types/game.types';
import styles from '@/components/game/vote-tally.module.css';

// The revealed vote tally, shown as a dismissible banner after an elimination
// (the full tally is revealed at elimination time, per ADR 0004). Dismissal is
// owned by the parent (it resets when a new tally arrives), so this component
// just emits `dismiss`.
defineProps<{
  tally: TallyDto | null;
  nameOf: (participantId: string) => string;
}>();

defineEmits<{ dismiss: [] }>();
</script>

<template>
  <section v-if="tally" :class="styles.banner" role="status">
    <div :class="styles.head">
      <div :class="styles.headInfo">
        <span :class="styles.eyebrow">Vote tally</span>
        <span :class="styles.meta">{{ tally.abstains }} abstain{{ tally.abstains === 1 ? '' : 's' }}</span>
      </div>
      <button type="button" :class="styles.close" aria-label="Dismiss tally" @click="$emit('dismiss')">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          aria-hidden="true"
        >
          <line x1="18" y1="6" x2="6" y2="18" />
          <line x1="6" y1="6" x2="18" y2="18" />
        </svg>
      </button>
    </div>
    <ul :class="styles.list">
      <li v-for="entry in tally.entries" :key="entry.participantId" :class="styles.row">
        <span :class="styles.name">{{ nameOf(entry.participantId) }}</span>
        <span :class="styles.barTrack" aria-hidden="true">
          <span :class="styles.bar" :style="{ width: `${Math.min(100, entry.count * 25)}%` }"></span>
        </span>
        <span :class="styles.count">{{ entry.count }}</span>
      </li>
    </ul>
  </section>
</template>