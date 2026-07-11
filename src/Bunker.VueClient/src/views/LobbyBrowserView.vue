<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useLobbyStore } from '@/stores/lobby.store';
import { useToast } from '@/composables/useToast';
import type { LobbySummary } from '@/types/lobby.types';
import LobbySummaryCard from '@/components/lobby/LobbySummaryCard.vue';
import LobbyIcons from '@/components/icons/LobbyIcons.vue';
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

// Join-by-code (relocated from the create page). Always available here so you
// can join a friend's game without first going to "New lobby".
const inviteCode = ref('');
const submittingCode = ref(false);

// Password-protected lobbies prompt for a password before joining.
const passwordTarget = ref<LobbySummary | null>(null);
const passwordInput = ref('');
const submittingPassword = ref(false);

// Joining a different lobby while already in one requires leaving first; we
// confirm before doing so (and warn the host that leaving destroys it). This
// is generic over both entry paths — a card join or a code join — so both just
// stash a `pendingJoin` action and reuse the same confirm modal.
const pendingJoin = ref<(() => Promise<void>) | null>(null);
const leavingLobby = ref(false);

onMounted(async () => {
  await run(() => lobbyStore.fetchBrowser());
});

async function enterRoom(lobbyId: string): Promise<void> {
  await router.push({ name: 'lobby-room', params: { id: lobbyId } });
}

// Actually join `summary` (by password or id) and navigate to it. Called either
// directly (when not in a lobby) or after leaving the current one. Public
// lobbies join by id — invite codes are private to the host now, so the
// browser list only knows lobby ids, never codes.
async function proceedJoin(summary: LobbySummary): Promise<void> {
  if (summary.hasPassword) {
    passwordInput.value = '';
    passwordTarget.value = summary;
    return;
  }
  const lobby = await run(() => lobbyStore.joinById(summary.id));
  if (lobby) await enterRoom(lobby.id);
}

async function openLobby(summary: LobbySummary): Promise<void> {
  // Joining another lobby while we're already in one would leak a ghost
  // participant (the backend doesn't enforce single-membership), so confirm
  // the leave first.
  if (currentLobbyId.value && summary.id !== currentLobbyId.value) {
    pendingJoin.value = () => proceedJoin(summary);
    return;
  }
  await proceedJoin(summary);
}

// Join a lobby by an invite code typed into the relocated panel. A code grants
// access even to password-protected lobbies, so (matching the old create-page
// behavior) we go straight to joinByCode with no separate password prompt.
async function joinByCode(): Promise<void> {
  const code = inviteCode.value.trim().toUpperCase();
  if (!code) return;
  // Same single-membership guard as a card join: confirm before leaving.
  if (currentLobbyId.value) {
    pendingJoin.value = () => runJoinCode(code);
    return;
  }
  await runJoinCode(code);
}

async function runJoinCode(code: string): Promise<void> {
  submittingCode.value = true;
  try {
    const lobby = await run(() => lobbyStore.joinByCode(code));
    if (lobby) {
      inviteCode.value = '';
      await enterRoom(lobby.id);
    }
  } finally {
    submittingCode.value = false;
  }
}

async function confirmLeaveAndJoin(): Promise<void> {
  const action = pendingJoin.value;
  if (!action) return;
  leavingLobby.value = true;
  try {
    const ok = await run(() => lobbyStore.leaveCurrent());
    if (ok === null) return; // leave failed; keep the user in their current lobby
    pendingJoin.value = null;
    await action();
  } finally {
    leavingLobby.value = false;
  }
}

function closeLeaveModal(): void {
  pendingJoin.value = null;
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
  <section :class="styles.page">
    <header :class="styles.pageHead">
      <div :class="styles.headText">
        <span :class="styles.eyebrow">Lobbies</span>
        <h1 :class="styles.title">Find a game</h1>
        <p :class="styles.subtitle">
          Join a public lobby in progress, enter an invite code, or host your own.
        </p>
      </div>
      <button :class="styles.newLobby" type="button" @click="router.push('/lobbies/new')">
        <LobbyIcons :class="styles.newLobbyIcon" name="plus" />
        New lobby
      </button>
    </header>

    <!-- Join with an invite code — relocated here from the create page so a
         player with a code doesn't have to open "New lobby" to find the field. -->
    <div :class="styles.joinPanel">
      <div :class="styles.joinLead">
        <span :class="styles.joinIcon"><LobbyIcons name="key" /></span>
        <div>
          <h2 :class="styles.joinTitle">Join with a code</h2>
          <p :class="styles.joinHint">
            Got an invite code from a friend? Enter it to jump straight in.
          </p>
        </div>
      </div>
      <form :class="styles.joinForm" @submit.prevent="joinByCode">
        <input
          v-model="inviteCode"
          :class="styles.joinInput"
          type="text"
          placeholder="ABC123"
          maxlength="16"
          spellcheck="false"
          autocapitalize="characters"
        />
        <button
          type="submit"
          :class="styles.joinBtn"
          :disabled="submittingCode || !inviteCode.trim()"
        >
          {{ submittingCode ? 'Joining…' : 'Join' }}
        </button>
      </form>
    </div>

    <div :class="styles.listHead">
      <h2 :class="styles.listTitle">
        <LobbyIcons :class="styles.listIcon" name="users" />
        Public lobbies
      </h2>
      <span :class="styles.listCount">{{ lobbyStore.summaries.length }} open</span>
    </div>

    <div v-if="lobbyStore.summaries.length === 0" :class="styles.empty">
      <span :class="styles.emptyIcon"><LobbyIcons name="sparkle" /></span>
      <p :class="styles.emptyTitle">No public lobbies yet</p>
      <p :class="styles.emptyBody">Be the first — host a game and share the invite code.</p>
      <button :class="styles.emptyCta" type="button" @click="router.push('/lobbies/new')">
        Host a game
      </button>
    </div>

    <ul v-else :class="styles.list">
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
      :open="pendingJoin !== null"
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
          <button type="button" :class="styles.passwordCancel" @click="closePasswordModal">
            Cancel
          </button>
          <button
            type="submit"
            :class="styles.passwordSubmit"
            :disabled="submittingPassword || !passwordInput"
          >
            {{ submittingPassword ? 'Joining…' : 'Join lobby' }}
          </button>
        </div>
      </form>
    </Modal>
  </section>
</template>