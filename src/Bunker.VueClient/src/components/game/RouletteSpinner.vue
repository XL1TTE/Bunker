<script setup lang="ts">
import styles from '@/components/game/roulette-spinner.module.css';

defineProps<{
  tiedParticipantIds: string[];
  nameOf: (participantId: string) => string;
}>();
</script>

<template>
  <section :class="styles.panel">
    <div :class="styles.panelHeader">
      <h2 :class="styles.panelTitle">Roulette</h2>
      <span :class="styles.panelMeta">Tie-break</span>
    </div>
    <div :class="styles.body">
      <div :class="styles.spinner" aria-hidden="true"></div>
      <p :class="styles.text">
        Tied players are being decided at random…
      </p>
      <ul :class="styles.tiedList">
        <li v-for="(id, i) in tiedParticipantIds" :key="id" :class="styles.tiedItem">
          <span :class="styles.tiedIndex">{{ i + 1 }}</span>
          <span :class="styles.tiedName">{{ nameOf(id) }}</span>
        </li>
      </ul>
    </div>
  </section>
</template>