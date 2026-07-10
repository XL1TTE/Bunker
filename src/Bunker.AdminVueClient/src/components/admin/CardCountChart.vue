<script setup lang="ts">
import { computed } from 'vue';
import styles from '@/components/admin/card-count-chart.module.css';

// Each row = one character-card type, one bar = its count. The largest count fills
// the track; the rest scale relative to it (zero-baseline — counts start at 0).
const props = defineProps<{
  entries: Array<{ label: string; count: number }>;
  loading?: boolean;
}>();

const max = computed(() => props.entries.reduce((m, e) => Math.max(m, e.count), 0));
const total = computed(() => props.entries.reduce((s, e) => s + e.count, 0));
const isEmpty = computed(() => !props.loading && total.value === 0);

// Width as a percentage of the max row. A tiny floor (2%) keeps a single card visible
// instead of a sub-pixel sliver, but stays honestly proportional.
function fillPercent(count: number): number {
  if (max.value === 0) return 0;
  return Math.max(count === 0 ? 0 : 2, (count / max.value) * 100);
}

// Screen-reader summary so the chart's data is available without sight.
const srSummary = computed(() => {
  if (props.loading) return 'Card counts are loading.';
  if (isEmpty.value) return 'No character cards have been added yet.';
  const parts = props.entries.map((e) => `${e.label}: ${e.count}`).join(', ');
  return `Character card counts. ${parts}. Total ${total.value}.`;
});
</script>

<template>
  <section :class="styles.panel" aria-labelledby="card-count-heading">
    <div :class="styles.head">
      <h2 id="card-count-heading" :class="styles.title">Card stock by type</h2>
      <span v-if="!loading && !isEmpty" :class="styles.total">{{ total }} total</span>
    </div>

    <p class="visually-hidden">{{ srSummary }}</p>

    <div v-if="loading" :class="styles.rows">
      <div v-for="i in 7" :key="i" :class="styles.row">
        <span :class="styles.skeletonLabel" />
        <span :class="styles.skeletonBar" />
      </div>
    </div>

    <div v-else-if="isEmpty" :class="styles.empty">
      <p :class="styles.emptyTitle">No cards yet</p>
      <p :class="styles.emptyBody">Pick a card type from the sidebar and add a few — they'll show up here as stock.</p>
    </div>

    <ul v-else :class="styles.rows">
      <li v-for="e in entries" :key="e.label" :class="styles.row">
        <span :class="styles.rowLabel">{{ e.label }}</span>
        <span :class="styles.track">
          <span :class="styles.fill" :style="{ width: fillPercent(e.count) + '%' }" />
        </span>
        <span :class="styles.count">{{ e.count }}</span>
      </li>
    </ul>
  </section>
</template>