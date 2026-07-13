<script setup lang="ts">
import type { BunkerCardDto } from '@/types/game.types';
import styles from '@/components/game/bunker-card-panel.module.css';

// Modal content for the Bunker briefing. Rendered inside <Modal title="Bunker
// briefing"> by GameRoomView — the Modal supplies the dialog chrome + title,
// so this component only lays out the three scenario fields.
defineProps<{ bunkerCard: BunkerCardDto | null }>();
</script>

<template>
  <div v-if="bunkerCard" :class="styles.fields">
    <div :class="styles.field">
      <span :class="styles.fieldLabel">Catastrophe</span>
      <p :class="styles.fieldValue">{{ bunkerCard.catastrophe }}</p>
    </div>
    <div :class="styles.field">
      <span :class="styles.fieldLabel">Survival duration</span>
      <p :class="styles.fieldValue">{{ bunkerCard.survivalDuration }}</p>
    </div>
    <div :class="styles.field">
      <span :class="styles.fieldLabel">Bunker environment</span>
      <p :class="styles.fieldValue">{{ bunkerCard.bunkerEnvironment }}</p>
    </div>
  </div>
  <p v-else :class="styles.empty">The bunker card will be revealed when the game starts.</p>
</template>