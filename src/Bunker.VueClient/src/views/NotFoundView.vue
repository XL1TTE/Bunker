<script setup lang="ts">
import { RouterLink } from 'vue-router';
import styles from '@/views/not-found.module.css';
</script>

<template>
  <section :class="styles.container">
    <p :class="styles.code">404</p>
    <h1 :class="styles.title">This page doesn't exist</h1>
    <p :class="styles.message">
      The bunker has nothing for this address. Maybe the door is somewhere else.
    </p>
    <RouterLink to="/" :class="styles.button">← Back home</RouterLink>
  </section>
</template>