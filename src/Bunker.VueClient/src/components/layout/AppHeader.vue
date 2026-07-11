<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink, useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';
import ThemeToggle from '@/components/common/ThemeToggle.vue';
import LogoutIcon from '@/components/icons/LogoutIcon.vue';
import styles from '@/components/layout/app-header.module.css';

defineProps<{ transparent?: boolean }>();

const authStore = useAuthStore();
const route = useRoute();

const initial = computed(() =>
  authStore.profile?.nickname?.[0]?.toUpperCase() ?? '?',
);
</script>

<template>
  <header :class="[styles.header, transparent && styles.transparent]">
    <div :class="styles.brandWrap">
      <span :class="styles.logoMark" aria-hidden="true">B</span>
      <RouterLink to="/" :class="styles.brand">Bunker</RouterLink>
    </div>
    <nav :class="styles.nav">
      <template v-if="authStore.profile">
        <RouterLink
          to="/lobbies"
          :class="[
            styles.navLink,
            route.path.startsWith('/lobbies') && !route.path.startsWith('/lobbies/new')
              ? styles.navLinkActive
              : '',
          ]"
        >
          Lobbies
        </RouterLink>
        <RouterLink
          to="/lobbies/new"
          :class="[styles.navLink, route.path === '/lobbies/new' ? styles.navLinkActive : '']"
        >
          Create
        </RouterLink>
      </template>
      <ThemeToggle />
      <!-- Three states, in priority order:
           1. initializing — profile is being fetched on boot; show a spinner
              (and crucially do NOT show the Sign in button, which would let the
              user trigger a duplicate Keycloak login while the fetch is mid-air).
           2. profile loaded — the signed-in user chip (click to log out).
           3. otherwise — logged out, show Sign in. -->
      <span
        v-if="authStore.initializing"
        :class="styles.authLoading"
        role="status"
        aria-live="polite"
      >
        <span :class="styles.spinner" aria-hidden="true"></span>
        <span :class="styles.authLoadingText">Signing in…</span>
      </span>
      <button
        v-else-if="!authStore.profile"
        :class="styles.primaryButton"
        @click="authStore.login()"
      >
        Sign in
      </button>
      <button
        v-else
        :class="styles.userChip"
        :title="`Logged in as ${authStore.profile.nickname}`"
        aria-label="Log out"
        @click="authStore.logout()"
      >
        <span :class="styles.avatar">{{ initial }}</span>
        <span :class="styles.userName">{{ authStore.profile.nickname }}</span>
        <LogoutIcon :class="styles.chipExit" />
      </button>
    </nav>
  </header>
</template>