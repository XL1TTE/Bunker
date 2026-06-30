<script setup lang="ts">
import { computed } from 'vue';
import type { LobbySummary } from '@/types/lobby.types';
import { useCatalogStore } from '@/stores/catalog.store';
import styles from '@/components/lobby/lobby-summary-card.module.css';

const props = defineProps<{ summary: LobbySummary }>();
defineEmits<{ join: [] }>();

const catalogStore = useCatalogStore();

const isFull = computed(() => props.summary.currentPlayers >= props.summary.capacity);
</script>

<template>
  <article :class="styles.card">
    <div :class="styles.headRow">
      <span :class="styles.code">{{ summary.inviteCode }}</span>
      <span :class="[styles.countWrap, isFull ? styles.countWrapFull : '']">
        {{ summary.currentPlayers }} / {{ summary.capacity }}
      </span>
    </div>
    <p :class="styles.host">
      Hosted by <span :class="styles.hostName">{{ summary.hostNickname }}</span>
    </p>
    <div :class="styles.packs">
      <span v-for="id in summary.selectedPackIds" :key="id" :class="styles.packBadge">
        {{ catalogStore.packById.get(id)?.title ?? id }}
      </span>
      <span v-if="summary.selectedPackIds.length === 0" :class="styles.muted">
        No packs selected
      </span>
    </div>
    <div :class="styles.footer">
      <span :class="styles.meta">{{ summary.id.slice(0, 8) }}</span>
      <button :class="styles.joinButton" :disabled="isFull" @click="$emit('join')">
        {{ isFull ? 'Full' : 'Join →' }}
      </button>
    </div>
  </article>
</template>