<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/game.store';
import { useToast } from '@/composables/useToast';
import { PHASE_LABELS } from '@/components/game/game-phases';
import RouletteSpinner from '@/components/game/RouletteSpinner.vue';
import styles from '@/components/game/action-panel.module.css';

// The action bar — a compact full-width strip below the game header that tells
// you what's happening right now and gives you the buttons to act when it's
// your turn. Emphasized (accent glow) when you have an action to take, quiet
// when it's just a status line. The vote tally is hoisted to the view as a
// dismissible banner, so it no longer lives here.

const gameStore = useGameStore();
const { run } = useToast();

const phase = computed(() => gameStore.phase);
const phaseLabel = computed(() => (phase.value ? PHASE_LABELS[phase.value] : '—'));
const isMyTurn = computed(() => gameStore.isMyTurn);
const currentTurnParticipant = computed(() => gameStore.currentTurnParticipant);
const myUnrevealedKinds = computed(() => gameStore.myUnrevealedAttributeKinds);
const voteable = computed(() => gameStore.voteableParticipants);
const hasVoted = computed(() =>
  gameStore.me ? gameStore.votedParticipantIds.includes(gameStore.me.id) : false,
);

const turnName = computed(() => currentTurnParticipant.value?.nickname ?? 'the next player');

// Buttons only appear when the user actually has an action to take (their
// reveal turn, or voting before they've cast). The bar emphasizes for these.
const revealButtons = computed(() =>
  phase.value === 'Reveal' && isMyTurn.value ? myUnrevealedKinds.value : [],
);
const voteButtons = computed(() => voteable.value);
const canVote = computed(() => phase.value === 'Voting' && isMyTurn.value && !hasVoted.value);
const hasActions = computed(() => revealButtons.value.length > 0 || canVote.value);

// A single, plain status line describing what to do or what's happening —
// written from the player's side of the screen, in the interface's voice.
const prompt = computed(() => {
  switch (phase.value) {
    case null:
      return 'Loading game state…';
    case 'Reveal':
      return isMyTurn.value
        ? revealButtons.value.length > 0
          ? 'Reveal one of your hidden traits.'
          : 'You have nothing left to reveal.'
        : `Waiting for ${turnName.value} to reveal a trait.`;
    case 'Voting':
      if (!isMyTurn.value) return 'Voting is open — cast your vote.';
      return hasVoted.value ? 'Vote cast — waiting for the others.' : 'Vote to eliminate someone.';
    case 'Discussion':
      return 'Chat is open — make your case in the game chat.';
    case 'DiscussionClosing':
      return 'Closing statements — make your final case in the chat.';
    case 'BunkerIntroduction':
      return 'The bunker scenario is being revealed — open the briefing.';
    case 'IntroDiscussion':
      return isMyTurn.value
        ? 'Your turn to introduce yourself — use the game chat.'
        : `Waiting for ${turnName.value} to introduce themselves.`;
    case 'Finished':
      return 'The game is over.';
    default:
      return `Waiting for ${turnName.value}…`;
  }
});

function nameOf(participantId: string): string {
  const p = gameStore.participants.find((x) => x.id === participantId);
  return p?.nickname ?? participantId.slice(0, 8);
}

async function reveal(kind: string): Promise<void> {
  await run(() => gameStore.revealAttribute(kind));
}

async function vote(targetId: string): Promise<void> {
  await run(() => gameStore.vote(targetId));
}
</script>

<template>
  <section :class="[styles.bar, hasActions ? styles.active : '']" aria-live="polite">
    <div :class="styles.info">
      <span :class="styles.phaseTag">{{ phaseLabel }}</span>
      <p :class="styles.prompt">{{ prompt }}</p>
    </div>

    <div :class="styles.actions">
      <!-- Roulette tie-break — the spinner IS the action for this phase. -->
      <RouletteSpinner
        v-if="phase === 'Roulette'"
        :tied-participant-ids="gameStore.rouletteTiedIds"
        :name-of="nameOf"
        :compact="true"
      />

      <!-- Reveal: your turn -->
      <template v-else-if="revealButtons.length > 0">
        <button
          v-for="kind in revealButtons"
          :key="kind"
          :class="styles.actionButton"
          @click="reveal(kind)"
        >
          {{ kind }}
        </button>
      </template>

      <!-- Voting: your turn, not yet cast -->
      <template v-else-if="canVote">
        <button
          v-for="p in voteButtons"
          :key="p.id"
          :class="styles.actionButton"
          @click="vote(p.id)"
        >
          {{ p.nickname }}
        </button>
      </template>

      <!-- Otherwise: no buttons, the status line carries the bar. -->
    </div>
  </section>
</template>