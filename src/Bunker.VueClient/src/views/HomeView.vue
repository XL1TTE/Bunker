<script setup lang="ts">
import { RouterLink } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';
import styles from '@/views/home.module.css';

const authStore = useAuthStore();
</script>

<template>
  <section :class="styles.hero">
    <span :class="styles.eyebrow">Survival social deduction</span>
    <h1 :class="styles.title">Bunker</h1>
    <p :class="styles.subtitle">
      The world ended. The bunker holds what it holds.
      Decide who stays, who gets thrown to the surface — and who you trust.
    </p>
    <div :class="styles.actions">
      <RouterLink
        v-if="authStore.profile"
        to="/lobbies"
        :class="styles.primaryButton"
      >
        Browse lobbies →
      </RouterLink>
      <RouterLink
        v-if="authStore.profile"
        to="/lobbies/new"
        :class="styles.ghostButton"
      >
        Host a game
      </RouterLink>
      <button v-else :class="styles.primaryButton" @click="authStore.login()">
        Log in to play
      </button>
    </div>
    <div :class="styles.metaRow">
      <span :class="styles.metaItem"><span :class="styles.metaDot"></span> Real-time lobby</span>
      <span :class="styles.metaItem">No download</span>
      <span :class="styles.metaItem">Free forever</span>
    </div>
  </section>
</template>