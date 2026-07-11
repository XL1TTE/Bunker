<script setup lang="ts">
import { computed } from 'vue';
import type { LobbySummary } from '@/types/lobby.types';
import { useCatalogStore } from '@/stores/catalog.store';
import LobbyIcons from '@/components/icons/LobbyIcons.vue';
import styles from '@/components/lobby/lobby-summary-card.module.css';

const props = defineProps<{ summary: LobbySummary; isCurrent?: boolean }>();
defineEmits<{ join: []; enter: [] }>();

const catalogStore = useCatalogStore();

const isFull = computed(() => props.summary.currentPlayers >= props.summary.capacity);
</script>

<template>
  <article :class="styles.card">
    <div :class="styles.headRow">
      <span :class="styles.name">{{ summary.name }}</span>
      <span
        v-if="summary.hasPassword"
        :class="styles.protected"
        title="Joining requires a password"
      >
        <LobbyIcons :class="styles.lockIcon" name="lock" />
        Protected
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
      <span :class="[styles.count, isFull && styles.countFull]">
        <LobbyIcons :class="styles.countIcon" name="users" />
        {{ summary.currentPlayers }} / {{ summary.capacity }}
      </span>
      <!-- The lobby we're already in: we can always re-enter it, even when full. -->
      <button v-if="isCurrent" :class="styles.enterBtn" @click="$emit('enter')">
        Enter →
      </button>
      <button v-else :class="styles.joinBtn" :disabled="isFull" @click="$emit('join')">
        {{ isFull ? 'Full' : 'Join →' }}
      </button>
    </div>
  </article>
</template>