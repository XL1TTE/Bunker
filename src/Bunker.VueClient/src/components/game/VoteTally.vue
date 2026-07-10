<script setup lang="ts">
import type { TallyDto } from '@/types/game.types';
import styles from '@/components/game/vote-tally.module.css';

const props = defineProps<{
  tally: TallyDto | null;
  nameOf: (participantId: string) => string;
}>();
</script>

<template>
  <section v-if="props.tally" :class="styles.panel">
    <div :class="styles.panelHeader">
      <h2 :class="styles.panelTitle">Vote tally</h2>
      <span :class="styles.panelMeta">
        {{ props.tally.abstains }} abstain{{ props.tally.abstains === 1 ? '' : 's' }}
      </span>
    </div>
    <ul :class="styles.list">
      <li v-for="entry in props.tally.entries" :key="entry.participantId" :class="styles.row">
        <span :class="styles.name">{{ props.nameOf(entry.participantId) }}</span>
        <span :class="styles.barTrack" aria-hidden="true">
          <span :class="styles.bar" :style="{ width: `${Math.min(100, entry.count * 25)}%` }"></span>
        </span>
        <span :class="styles.count">{{ entry.count }}</span>
      </li>
    </ul>
  </section>
</template>