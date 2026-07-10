<script setup lang="ts">
import styles from '@/components/admin/stat-tile.module.css';

defineProps<{
  label: string;
  value: number | string;
  hint?: string;
  loading?: boolean;
}>();
</script>

<template>
  <div :class="styles.tile">
    <span :class="styles.label">{{ label }}</span>
    <span v-if="loading" :class="styles.skeleton" aria-label="Loading" />
    <span v-else :class="styles.value">{{ value }}</span>
    <span v-if="hint" :class="styles.hint">{{ hint }}</span>
  </div>
</template>