import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { PlayerProfile } from '@/types/account.types';
import { getApiContainer } from '@/api/register';
import { auth } from '@/auth/keycloak';

export const useAuthStore = defineStore('auth', () => {
  const profile = ref<PlayerProfile | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

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

  return { profile, loading, error, login, logout, fetchProfile, updateNickname };
});