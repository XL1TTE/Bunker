<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useLobbyStore } from '@/stores/lobby.store';
import { useCatalogStore } from '@/stores/catalog.store';
import { useAuthStore } from '@/stores/auth.store';
import ParticipantSlot from '@/components/lobby/ParticipantSlot.vue';
import SettingsPanel from '@/components/lobby/SettingsPanel.vue';
import ChatPanel from '@/components/lobby/ChatPanel.vue';
import styles from '@/views/lobby-room.module.css';

const props = defineProps<{ id: string }>();
const router = useRouter();
const lobbyStore = useLobbyStore();
const catalogStore = useCatalogStore();
const authStore = useAuthStore();

const showDestroyedModal = ref(false);
const showHandoffModal = ref(false);
const codeCopied = ref(false);

const lobby = computed(() => lobbyStore.currentLobby);
const emptySlots = computed(() => {
  const l = lobby.value;
  if (!l) return 0;
  return Math.max(0, l.capacity - l.participants.length);
});

const myParticipant = computed(() =>
  lobby.value?.participants.find((p) => p.accountId === authStore.profile?.id),
);
const isReady = computed(() => myParticipant.value?.status === 'Ready');

onMounted(async () => {
  await catalogStore.load();
  await authStore.fetchProfile();
  lobbyStore.setMyAccountId(authStore.profile?.id ?? null);
  await lobbyStore.fetchLobby(props.id);
  await lobbyStore.connectAndJoin(props.id);
});

onBeforeUnmount(async () => {
  await lobbyStore.disconnectRealtime();
  lobbyStore.setMyAccountId(null);
});

watch(
  () => lobbyStore.destroyed,
  (val) => {
    if (val) showDestroyedModal.value = true;
  },
);

watch(
  () => lobbyStore.handoffGameId,
  (val) => {
    if (val) showHandoffModal.value = true;
  },
);

async function leave(): Promise<void> {
  await lobbyStore.leaveCurrent();
  await router.push('/lobbies');
}

async function goToLobbies(): Promise<void> {
  showDestroyedModal.value = false;
  await router.push('/lobbies');
}

async function copyCode(): Promise<void> {
  const code = lobby.value?.inviteCode;
  if (!code) return;
  try {
    await navigator.clipboard.writeText(code);
    codeCopied.value = true;
    setTimeout(() => { codeCopied.value = false; }, 1500);
  } catch {
    codeCopied.value = false;
  }
}
</script>

<template>
  <section :class="styles.container">
    <header :class="styles.header">
      <div :class="styles.headerLeft">
        <div :class="styles.headerTitle">
          <span :class="styles.headerCode">{{ lobby?.inviteCode ?? '———' }}</span>
        </div>
        <p :class="styles.headerSubline">
          <span :class="styles.headerMetaItem">
            {{ lobby?.participants.length ?? 0 }} / {{ lobby?.capacity ?? '?' }} players
          </span>
          <span :class="styles.metaSep" aria-hidden="true"></span>
          <span :class="styles.headerMetaItem">
            <span
              :class="[
                styles.privacyTag,
                lobby?.isPublic ? styles.privacyPublic : styles.privacyPrivate,
              ]"
            >
              {{ lobby?.isPublic ? 'Public' : 'Private' }}
            </span>
          </span>
        </p>
      </div>
      <div :class="styles.headerRight">
        <button
          :class="[styles.copyButton, codeCopied ? styles.copyButtonCopied : '']"
          :disabled="!lobby?.inviteCode"
          @click="copyCode"
        >
          {{ codeCopied ? '✓ Copied' : 'Copy invite code' }}
        </button>
        <button :class="styles.leaveButton" @click="leave">Leave</button>
      </div>
    </header>

    <div :class="styles.grid">
      <div :class="styles.column">
        <div :class="styles.panel">
          <div :class="styles.panelHeader">
            <h2 :class="styles.panelTitle">Participants</h2>
            <span :class="styles.panelMeta">{{ lobby?.participants.length ?? 0 }}/{{ lobby?.capacity ?? '?' }}</span>
          </div>
          <div :class="styles.panelBody">
            <ul :class="styles.slotList">
              <li v-for="p in lobby?.participants ?? []" :key="p.id">
                <ParticipantSlot
                  :participant="p"
                  :is-host="lobbyStore.isHost"
                  :self-account-id="authStore.profile?.id"
                  :host-participant-id="lobby?.hostParticipantId ?? ''"
                  @kick="lobbyStore.kickParticipant"
                  @remove-bot="lobbyStore.removeBot"
                />
              </li>
              <li
                v-for="n in emptySlots"
                :key="`empty-${n}`"
                :class="styles.emptySlot"
              >
                Open slot
              </li>
            </ul>
          </div>
        </div>

        <div v-if="!lobbyStore.isHost" :class="styles.panel">
          <div :class="styles.panelHeader">
            <h2 :class="styles.panelTitle">Your status</h2>
          </div>
          <div :class="styles.readyBlock">
            <button
              :class="[styles.readyButton, isReady ? styles.readyButtonReady : '']"
              :disabled="!lobby"
              @click="lobbyStore.toggleReadiness"
            >
              {{ isReady ? '✓ Ready' : 'Mark as ready' }}
            </button>
            <p :class="styles.readyNote">
              Ready up when you've settled in. The host will start when everyone's ready.
            </p>
          </div>
        </div>

        <SettingsPanel v-if="lobbyStore.isHost" />
      </div>

      <div :class="styles.column">
        <ChatPanel />
      </div>
    </div>

    <Teleport to="body">
      <div v-if="showDestroyedModal" :class="styles.modalBackdrop" @click.self="goToLobbies">
        <div :class="styles.modal" role="dialog" aria-modal="true">
          <h2>Lobby was destroyed</h2>
          <p>The host left or the lobby was otherwise terminated.</p>
          <div :class="styles.modalActions">
            <button :class="styles.modalButton" @click="goToLobbies">Back to browser</button>
          </div>
        </div>
      </div>

      <div v-if="showHandoffModal" :class="styles.modalBackdrop">
        <div :class="styles.modal" role="dialog" aria-modal="true">
          <h2>Game starting…</h2>
          <p>Game session ID:</p>
          <code :class="styles.modalGameId">{{ lobbyStore.handoffGameId }}</code>
          <p :class="styles.modalFooter">
            The game screen is out of MVP scope — this is where it would launch.
          </p>
        </div>
      </div>
    </Teleport>
  </section>
</template>