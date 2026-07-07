<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useLobbyStore } from '@/stores/lobby.store';
import { useToast } from '@/composables/useToast';
import type { LobbySummary } from '@/types/lobby.types';
import LobbySummaryCard from '@/components/lobby/LobbySummaryCard.vue';
import Modal from '@/components/common/Modal.vue';
import styles from '@/views/lobby-browser.module.css';

const router = useRouter();
const lobbyStore = useLobbyStore();
const { run } = useToast();

// Password-protected lobbies prompt for a password before joining.
const passwordTarget = ref<LobbySummary | null>(null);
const passwordInput = ref('');
const submittingPassword = ref(false);

onMounted(async () => {
  await run(() => lobbyStore.fetchBrowser());
});

async function enterRoom(lobbyId: string): Promise<void> {
  await router.push({ name: 'lobby-room', params: { id: lobbyId } });
}

async function openLobby(summary: LobbySummary): Promise<void> {
  if (summary.hasPassword) {
    passwordInput.value = '';
    passwordTarget.value = summary;
    return;
  }
  // Public, no password: join by invite code, then enter the room.
  const lobby = await run(() => lobbyStore.joinByCode(summary.inviteCode));
  if (lobby) await enterRoom(lobby.id);
}

async function submitPassword(): Promise<void> {
  const target = passwordTarget.value;
  const password = passwordInput.value;
  if (!target || !password) return;
  submittingPassword.value = true;
  try {
    const lobby = await run(() => lobbyStore.joinByPassword(target.id, password));
    if (lobby) {
      passwordTarget.value = null;
      passwordInput.value = '';
      await enterRoom(lobby.id);
    }
  } finally {
    submittingPassword.value = false;
  }
}

function closePasswordModal(): void {
  passwordTarget.value = null;
  passwordInput.value = '';
}
</script>

<template>
  <section :class="styles.container">
    <div :class="styles.headingBlock">
      <div>
        <h1 :class="styles.title">Public lobbies</h1>
        <p :class="styles.subtitle">Pick a game in progress — or host your own.</p>
      </div>
      <div :class="styles.toolbar">
        <button :class="styles.newButton" @click="router.push('/lobbies/new')">
          + New lobby
        </button>
      </div>
    </div>

    <div v-if="lobbyStore.summaries.length === 0" :class="styles.empty">
      <p :class="styles.emptyTitle">No public lobbies yet</p>
      <p>Be the first. Spin one up and your friends can join with the invite code.</p>
    </div>

    <ul :class="styles.list">
      <li v-for="summary in lobbyStore.summaries" :key="summary.id">
        <LobbySummaryCard :summary="summary" @join="openLobby(summary)" />
      </li>
    </ul>

    <Modal
      :open="passwordTarget !== null"
      title="Enter lobby password"
      @close="closePasswordModal"
    >
      <p v-if="passwordTarget" :class="styles.passwordHint">
        Joining <strong>{{ passwordTarget.hostNickname }}</strong>'s lobby.
      </p>
      <form :class="styles.passwordForm" @submit.prevent="submitPassword">
        <input
          v-model="passwordInput"
          type="password"
          placeholder="Password"
          autocomplete="current-password"
          autofocus
        />
        <div :class="styles.passwordActions">
          <button type="button" :class="styles.passwordCancel" @click="closePasswordModal">Cancel</button>
          <button type="submit" :class="styles.passwordSubmit" :disabled="submittingPassword || !passwordInput">
            {{ submittingPassword ? 'Joining…' : 'Join lobby' }}
          </button>
        </div>
      </form>
    </Modal>
  </section>
</template>