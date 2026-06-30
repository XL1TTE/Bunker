<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { RouterLink } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';
import { auth } from '@/auth/keycloak';
import styles from '@/styles/app.module.css';

const authStore = useAuthStore();

const initial = computed(() =>
  authStore.profile?.nickname?.[0]?.toUpperCase() ?? '?',
);

onMounted(async () => {
  if (auth.isAuthenticated()) {
    await authStore.fetchProfile();
  }
});
</script>

<template>
  <div :class="styles.shell">
    <header :class="styles.header">
      <div :class="styles.brandWrap">
        <span :class="styles.logoMark" aria-hidden="true">B</span>
        <RouterLink to="/" :class="styles.brand">Bunker</RouterLink>
      </div>
      <nav :class="styles.nav">
        <template v-if="authStore.profile">
          <RouterLink
            to="/lobbies"
            :class="[styles.navLink, $route.path.startsWith('/lobbies') && !$route.path.startsWith('/lobbies/new') ? styles.navLinkActive : '']"
          >
            Lobbies
          </RouterLink>
          <RouterLink
            to="/lobbies/new"
            :class="[styles.navLink, $route.path === '/lobbies/new' ? styles.navLinkActive : '']"
          >
            Create
          </RouterLink>
          <button :class="styles.userChip" @click="authStore.logout()" :title="`Logged in as ${authStore.profile.nickname}`">
            <span :class="styles.avatar">{{ initial }}</span>
            <span>{{ authStore.profile.nickname }}</span>
            <span :class="styles.chipExit" aria-hidden="true">↩</span>
          </button>
        </template>
        <button v-else :class="styles.primaryButton" @click="authStore.login()">
          Log in
        </button>
      </nav>
    </header>
    <main :class="styles.main">
      <RouterView />
    </main>
  </div>
</template>