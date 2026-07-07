<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
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

// The lobby we're currently a participant in (null when we're in none). Drives
// the "Enter" action on our own card and the leave-and-join prompt on others.
const currentLobbyId = computed(() => lobbyStore.currentLobbyId);
const currentLobbyHost = computed(() => {
  const l = lobbyStore.currentLobby;
  if (!l) return null;
  const host = l.participants.find((p) => p.id === l.hostParticipantId);
  return host?.nickname ?? null;
});

// Password-protected lobbies prompt for a password before joining.
const passwordTarget = ref<LobbySummary | null>(null);
const passwordInput = ref('');
const submittingPassword = ref(false);

// Joining a different lobby while already in one requires leaving first; we
// confirm before doing so (and warn the host that leaving destroys it).
const leaveTarget = ref<LobbySummary | null>(null);
const leavingLobby = ref(false);

onMounted(async () => {
  await run(() => lobbyStore.fetchBrowser());
});

async function enterRoom(lobbyId: string): Promise<void> {
  await router.push({ name: 'lobby-room', params: { id: lobbyId } });
}

// Actually join `summary` (by password or invite code) and navigate to it.
// Called either directly (when not in a lobby) or after leaving the current one.
async function proceedJoin(summary: LobbySummary): Promise<void> {
  if (summary.hasPassword) {
    passwordInput.value = '';
    passwordTarget.value = summary;
    return;
  }
  const lobby = await run(() => lobbyStore.joinByCode(summary.inviteCode));
  if (lobby) await enterRoom(lobby.id);
}

async function openLobby(summary: LobbySummary): Promise<void> {
  // Joining another lobby while we're already in one would leak a ghost
  // participant (the backend doesn't enforce single-membership), so confirm
  // the leave first.
  if (currentLobbyId.value && summary.id !== currentLobbyId.value) {
    leaveTarget.value = summary;
    return;
  }
  await proceedJoin(summary);
}

async function confirmLeaveAndJoin(): Promise<void> {
  const target = leaveTarget.value;
  if (!target) return;
  leavingLobby.value = true;
  try {
    const ok = await run(() => lobbyStore.leaveCurrent());
    if (ok === null) return; // leave failed; keep the user in their current lobby
    leaveTarget.value = null;
    await proceedJoin(target);
  } finally {
    leavingLobby.value = false;
  }
}

function closeLeaveModal(): void {
  leaveTarget.value = null;
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
        <LobbySummaryCard
          :summary="summary"
          :is-current="summary.id === currentLobbyId"
          @join="openLobby(summary)"
          @enter="enterRoom(summary.id)"
        />
      </li>
    </ul>

    <Modal
      :open="leaveTarget !== null"
      title="Leave your current lobby?"
      @close="closeLeaveModal"
    >
      <p :class="styles.passwordHint">
        You're already in <strong>{{ currentLobbyHost ?? 'a lobby' }}</strong>'s lobby.
        Joining another one means leaving this one first.
      </p>
      <p v-if="lobbyStore.isHost" :class="styles.passwordHint">
        You're the host — leaving will destroy the current lobby for everyone.
      </p>
      <div :class="styles.passwordActions">
        <button
          type="button"
          :class="styles.passwordCancel"
          :disabled="leavingLobby"
          @click="closeLeaveModal"
        >
          Stay here
        </button>
        <button
          type="button"
          :class="styles.passwordSubmit"
          :disabled="leavingLobby"
          @click="confirmLeaveAndJoin"
        >
          {{ leavingLobby ? 'Leaving…' : 'Leave & join' }}
        </button>
      </div>
    </Modal>

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