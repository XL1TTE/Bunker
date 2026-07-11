<script setup lang="ts">
import { computed } from 'vue';
import { useThemeStore } from '@/stores/theme.store';
import SunIcon from '@/components/icons/SunIcon.vue';
import MoonIcon from '@/components/icons/MoonIcon.vue';
import styles from '@/components/common/theme-toggle.module.css';

const themeStore = useThemeStore();
const isLight = computed(() => themeStore.theme === 'light');
</script>

<template>
  <button
    type="button"
    :class="styles.toggle"
    :aria-label="isLight ? 'Switch to dark theme' : 'Switch to light theme'"
    :aria-pressed="isLight"
    :title="isLight ? 'Switch to dark theme' : 'Switch to light theme'"
    @click="themeStore.toggle()"
  >
    <SunIcon v-if="isLight" :class="styles.icon" />
    <MoonIcon v-else :class="styles.icon" />
  </button>
</template>