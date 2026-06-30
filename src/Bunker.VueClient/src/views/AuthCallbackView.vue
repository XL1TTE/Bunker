<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { auth } from '@/auth/keycloak';
import styles from '@/views/auth-callback.module.css';

const route = useRoute();
const router = useRouter();
const error = ref<string | null>(null);

onMounted(async () => {
  try {
    await auth.init();
    if (auth.isAuthenticated()) {
      const redirect = (route.query.redirect as string | undefined) ?? '/lobbies';
      await router.replace(redirect);
    } else {
      error.value = 'Authentication did not complete.';
    }
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Unknown error.';
  }
});
</script>

<template>
  <div :class="styles.container">
    <span v-if="!error" :class="styles.spinner" aria-hidden="true"></span>
    <p v-if="!error">Completing sign-in…</p>
    <p v-else :class="styles.error">Sign-in failed: {{ error }}</p>
  </div>
</template>