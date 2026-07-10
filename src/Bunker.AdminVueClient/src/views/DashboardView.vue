<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useAdminStore } from '@/stores/admin.store';
import { useToast } from '@/composables/useToast';
import { CARD_TYPES } from '@/types/admin.types';
import StatTile from '@/components/admin/StatTile.vue';
import CardCountChart from '@/components/admin/CardCountChart.vue';
import styles from '@/views/page.module.css';

const admin = useAdminStore();
const { run } = useToast();

onMounted(async () => {
  await run(() => admin.loadDashboard(), { message: 'Could not load the dashboard.' });
});

const stats = computed(() => admin.dashboard);
const totalCards = computed(() =>
  stats.value ? Object.values(stats.value.cardCounts).reduce((s, n) => s + n, 0) : 0,
);

const chartEntries = computed(() =>
  CARD_TYPES.map((t) => ({ label: t.label, count: stats.value?.cardCounts[t.key] ?? 0 })),
);
</script>

<template>
  <section :class="styles.page">
    <div :class="styles.heading">
      <h1 :class="styles.title">Dashboard</h1>
      <p :class="styles.subtitle">A quick read on what's in the deck.</p>
    </div>

    <div :class="styles.tiles">
      <StatTile label="Character cards" :value="totalCards" :loading="admin.dashboardLoading" hint="across 7 types" />
      <StatTile label="Bunker cards" :value="stats?.bunkerCount ?? 0" :loading="admin.dashboardLoading" />
      <StatTile label="Card packs" :value="stats?.packCount ?? 0" :loading="admin.dashboardLoading" />
      <StatTile label="Bot presets" :value="stats?.presetCount ?? 0" :loading="admin.dashboardLoading" />
    </div>

    <CardCountChart :entries="chartEntries" :loading="admin.dashboardLoading" />
  </section>
</template>