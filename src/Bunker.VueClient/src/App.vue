<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';
import { useToastStore } from '@/stores/toast.store';
import AppHeader from '@/components/layout/AppHeader.vue';
import ToastHost from '@/components/common/ToastHost.vue';
import styles from '@/styles/app.module.css';

const authStore = useAuthStore();
const toast = useToastStore();
const route = useRoute();

// Landing route renders full-bleed under a transparent header; every other
// route keeps the solid sticky header + max-width main.
const isLanding = computed(() => route.meta.layout === 'landing');

// Resolve the signed-in user's profile on boot. While this is in flight the
// auth store's `initializing` flag is true, so the header shows a "signing in"
// spinner in place of the Sign in button (and the landing CTAs are disabled) —
// the user never sees a logged-out-looking page for an account they're already
// authenticated as. If the fetch fails, surface a toast so it isn't silent
// (the header then falls back to Sign in, which the user can click to retry).
onMounted(async () => {
  await authStore.initialize();
  if (authStore.error) {
    toast.error('Couldn’t load your profile. Sign in to try again.');
  }
});
</script>

<template>
  <div :class="styles.shell">
    <AppHeader :transparent="isLanding" />
    <main :class="[styles.main, isLanding && styles.mainLanding]">
      <RouterView />
    </main>

    <ToastHost />
  </div>
</template>