<script setup lang="ts">
import type { BunkerCardDto } from '@/types/game.types';
import styles from '@/components/game/bunker-card-panel.module.css';

defineProps<{ bunkerCard: BunkerCardDto | null }>();
</script>

<template>
  <section :class="styles.panel">
    <div :class="styles.panelHeader">
      <h2 :class="styles.panelTitle">Bunker</h2>
      <span v-if="bunkerCard" :class="styles.panelMeta">The scenario</span>
    </div>
    <div v-if="bunkerCard" :class="styles.body">
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
  </section>
</template>