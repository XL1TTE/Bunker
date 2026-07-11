import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { PlayerProfile } from '@/types/account.types';
import { getApiContainer } from '@/api/register';
import { auth } from '@/auth/keycloak';

export const useAuthStore = defineStore('auth', () => {
  const profile = ref<PlayerProfile | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // True while the app is resolving the signed-in user's identity on boot —
  // i.e. between "Keycloak says you're authenticated" and "we've fetched your
  // /accounts/me profile". Seeded from auth.isAuthenticated() so it is already
  // true on the very first render for an authenticated user (no "Sign in"
  // flash before the profile arrives). Stays false for logged-out users (no
  // fetch happens, so no spinner). UI uses this — not `loading` — to decide
  // whether to show a "signing in" state and hide the Sign in entry points:
  // `loading` also toggles on later profile refreshes (e.g. LobbyRoomView),
  // which must NOT show a "signing in" spinner.
  const initializing = ref(auth.isAuthenticated());

  async function login(): Promise<void> {
    await auth.login();
  }

  async function logout(): Promise<void> {
    await auth.logout();
    profile.value = null;
  }

  async function fetchProfile(): Promise<void> {
    loading.value = true;
    error.value = null;
    try {
      profile.value = await getApiContainer().account.getMyProfile();
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to load profile.';
    } finally {
      loading.value = false;
    }
  }

  async function updateNickname(nickname: string): Promise<void> {
    profile.value = await getApiContainer().account.updateMyNickname(nickname);
  }

  // Boot-time resolution. Called once from App.vue onMounted. For a logged-out
  // user there is nothing to fetch, so it just clears `initializing`. For a
  // logged-in user it fetches the profile (driving `loading`); either way it
  // drops `initializing` in `finally` so the UI never gets stuck in the
  // "signing in" state — not even if the fetch throws.
  async function initialize(): Promise<void> {
    if (!auth.isAuthenticated()) {
      initializing.value = false;
      return;
    }
    try {
      await fetchProfile();
    } finally {
      initializing.value = false;
    }
  }

  return {
    profile,
    loading,
    error,
    initializing,
    login,
    logout,
    fetchProfile,
    initialize,
    updateNickname,
  };
});