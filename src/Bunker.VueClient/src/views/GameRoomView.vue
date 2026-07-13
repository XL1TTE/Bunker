<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useGameStore } from '@/stores/game.store';
import { useLobbyStore } from '@/stores/lobby.store';
import { useToast } from '@/composables/useToast';
import BunkerCardPanel from '@/components/game/BunkerCardPanel.vue';
import CharacterSheet from '@/components/game/CharacterSheet.vue';
import ActionPanel from '@/components/game/ActionPanel.vue';
import ChatPanel from '@/components/game/ChatPanel.vue';
import VoteTally from '@/components/game/VoteTally.vue';
import PhaseStepper from '@/components/game/PhaseStepper.vue';
import PhaseChangeModal from '@/components/game/PhaseChangeModal.vue';
import Modal from '@/components/common/Modal.vue';
import BriefingIcon from '@/components/icons/BriefingIcon.vue';
import styles from '@/views/game-room.module.css';

const props = defineProps<{ gameId: string }>();
const router = useRouter();
const gameStore = useGameStore();
const lobbyStore = useLobbyStore();
const { run } = useToast();

const game = computed(() => gameStore.currentGame);
const aliveCount = computed(() => gameStore.aliveCount);

const now = ref(Date.now());
let tickId: number | null = null;
const remainingMs = computed(() =>
  gameStore.phaseDeadline ? Math.max(0, gameStore.phaseDeadline - now.value) : null,
);
const remainingLabel = computed(() => {
  const ms = remainingMs.value;
  if (ms == null) return null;
  const total = Math.ceil(ms / 1000);
  const m = Math.floor(total / 60);
  const s = total % 60;
  return `${m}:${String(s).padStart(2, '0')}`;
});
const survivorNames = computed(() =>
  gameStore.survivors.map((id) => nameOf(id)),
);

function isCurrentTurn(participantId: string): boolean {
  return gameStore.currentTurnParticipantId === participantId;
}

function nameOf(participantId: string): string {
  const p = gameStore.participants.find((x) => x.id === participantId);
  return p?.nickname ?? participantId.slice(0, 8);
}

// Bunker briefing: auto-open the modal once when the BunkerIntroduction phase
// begins (the card is being revealed to everyone), then hide it under the
// header button so it can be re-read on demand. `bunkerAutoShown` resets when
// the phase leaves BunkerIntroduction, so a later briefing re-opens.
const showBunkerModal = ref(false);
const bunkerAutoShown = ref(false);
const bunkerAvailable = computed(() => !!gameStore.bunkerCard);

watch(
  () => gameStore.phase,
  (phase) => {
    if (phase === 'BunkerIntroduction' && bunkerAvailable.value && !bunkerAutoShown.value) {
      showBunkerModal.value = true;
      bunkerAutoShown.value = true;
    } else if (phase !== 'BunkerIntroduction') {
      bunkerAutoShown.value = false;
    }
  },
);

// The card can arrive slightly after the phase flips to BunkerIntroduction
// (via the BunkerCardRevealed event). Re-evaluate when the card lands so the
// modal still auto-opens for that briefing.
watch(
  () => bunkerAvailable.value,
  (available) => {
    if (available && gameStore.phase === 'BunkerIntroduction' && !bunkerAutoShown.value) {
      showBunkerModal.value = true;
      bunkerAutoShown.value = true;
    }
  },
);

function openBunker(): void {
  showBunkerModal.value = true;
}
function closeBunker(): void {
  showBunkerModal.value = false;
}

// Chat sidebar collapse — give the participant grid the full width when the
// user wants the space. A floating pill re-opens it.
const chatCollapsed = ref(false);

// Vote tally banner — show when a fresh tally arrives (after an elimination);
// let the user dismiss it, and reset the dismissal when a new tally comes in.
const tallyDismissed = ref(false);
watch(
  () => gameStore.lastTally,
  (tally) => {
    if (tally) tallyDismissed.value = false;
  },
);

onMounted(async () => {
  tickId = window.setInterval(() => {
    now.value = Date.now();
  }, 1000);

  const snapshot = await run(() => gameStore.fetchGame(props.gameId));
  if (!snapshot) {
    // 403 (not a participant) / 404 / network — head back to the browser.
    await router.replace('/lobbies');
    return;
  }
  await run(() => gameStore.fetchMessages(props.gameId));
  await run(() => gameStore.connectAndJoin(props.gameId));
});

onBeforeUnmount(() => {
  if (tickId !== null) {
    window.clearInterval(tickId);
    tickId = null;
  }
  gameStore.disconnectRealtime();
  // Clear finished/survivors so a subsequent game (the same stack starting
  // another) mounts clean — the Game-over modal won't flash the prior result.
  gameStore.reset();
});

async function leaveGame(): Promise<void> {
  const ok = await run(() => gameStore.leaveGame());
  if (ok === null) return;
  lobbyStore.reset();
  await router.push('/lobbies');
}

// After the game finishes the lobby reopens — the backend resets it to
// WaitingForPlayers with the same participants (GameFinishedHandler ->
// ReopenAfterGame), and we never left it (the handoff only tore down lobby
// realtime, not membership). So send everyone back to the lobby room, not the
// browser, so the same stack can ready up and start another game or leave. The
// lobby id isn't on the game snapshot; it persists in the lobby store from
// before the handoff. If it's missing (e.g. a deep-link reload lost Pinia
// state), fall back to the browser.
async function backToLobby(): Promise<void> {
  const lobbyId = lobbyStore.currentLobbyId;
  if (lobbyId) {
    await router.push({ name: 'lobby-room', params: { id: lobbyId } });
  } else {
    await router.push('/lobbies');
  }
}
</script>

<template>
  <section :class="styles.container">
    <header :class="styles.gameHeader">
      <div :class="styles.headerLeft">
        <span :class="styles.roundBadge">Round {{ game?.roundNumber ?? '—' }}</span>
        <PhaseStepper />
        <span v-if="remainingLabel" :class="styles.timerChip">{{ remainingLabel }}</span>
        <span :class="styles.aliveMeta">{{ aliveCount }} / {{ game?.bunkerCapacity ?? '—' }} to survive</span>
      </div>
      <div :class="styles.headerRight">
        <button
          :class="styles.briefingButton"
          :disabled="!bunkerAvailable"
          :title="bunkerAvailable ? 'Open the bunker briefing' : 'The bunker card will be revealed when the game starts'"
          @click="openBunker"
        >
          <BriefingIcon :class="styles.briefingIcon" />
          Bunker briefing
        </button>
        <button :class="styles.leaveButton" @click="leaveGame">Leave game</button>
      </div>
    </header>

    <ActionPanel />

    <VoteTally
      v-if="gameStore.lastTally && !tallyDismissed"
      :tally="gameStore.lastTally"
      :name-of="nameOf"
      @dismiss="tallyDismissed = true"
    />

    <div :class="[styles.contentRow, chatCollapsed ? styles.chatCollapsed : '']">
      <div :class="styles.participantsColumn">
        <ul :class="styles.sheetList">
          <li v-for="p in gameStore.participants" :key="p.id">
            <CharacterSheet :participant="p" :is-current-turn="isCurrentTurn(p.id)" />
          </li>
        </ul>
      </div>
      <div :class="styles.chatColumn">
        <ChatPanel @collapse="chatCollapsed = true" />
      </div>
    </div>

    <button v-if="chatCollapsed" :class="styles.showChatPill" type="button" @click="chatCollapsed = false">
      <svg
        xmlns="http://www.w3.org/2000/svg"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        stroke-linecap="round"
        stroke-linejoin="round"
        aria-hidden="true"
      >
        <path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z" />
      </svg>
      Show chat
    </button>

    <Modal :open="showBunkerModal" title="Bunker briefing" @close="closeBunker">
      <BunkerCardPanel :bunker-card="gameStore.bunkerCard" />
    </Modal>

    <Modal :open="gameStore.finished" title="Game over" @close="backToLobby">
      <p>These players made it into the bunker:</p>
      <ul :class="styles.survivorList">
        <li v-for="name in survivorNames" :key="name" :class="styles.survivorItem">{{ name }}</li>
      </ul>
      <div :class="styles.modalActions">
        <button :class="styles.modalButton" @click="backToLobby">Back to lobby</button>
      </div>
    </Modal>

    <PhaseChangeModal />
  </section>
</template>