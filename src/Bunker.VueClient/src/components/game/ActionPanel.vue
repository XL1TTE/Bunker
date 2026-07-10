<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/game.store';
import { useToast } from '@/composables/useToast';
import type { GamePhase } from '@/types/game.types';
import VoteTally from '@/components/game/VoteTally.vue';
import RouletteSpinner from '@/components/game/RouletteSpinner.vue';
import styles from '@/components/game/action-panel.module.css';

const gameStore = useGameStore();
const { run } = useToast();

const PHASE_LABELS: Record<GamePhase, string> = {
  BunkerIntroduction: 'Bunker briefing',
  IntroDiscussion: 'Introductions',
  Reveal: 'Reveal',
  Discussion: 'Discussion',
  DiscussionClosing: 'Closing statements',
  Voting: 'Voting',
  Roulette: 'Roulette',
  Finished: 'Finished',
};

const phase = computed(() => gameStore.phase);
const phaseLabel = computed(() => (phase.value ? PHASE_LABELS[phase.value] : '—'));
const isMyTurn = computed(() => gameStore.isMyTurn);
const currentTurnParticipant = computed(() => gameStore.currentTurnParticipant);
const myUnrevealedKinds = computed(() => gameStore.myUnrevealedAttributeKinds);
const voteable = computed(() => gameStore.voteableParticipants);
const hasVoted = computed(() =>
  gameStore.me ? gameStore.votedParticipantIds.includes(gameStore.me.id) : false,
);

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
  <section :class="styles.panel">
    <div :class="styles.panelHeader">
      <h2 :class="styles.panelTitle">Your move</h2>
      <span :class="styles.panelMeta">{{ phaseLabel }}</span>
    </div>

    <div :class="styles.body">
      <!-- Loading / no game -->
      <p v-if="!phase" :class="styles.status">Loading game state…</p>

      <!-- Reveal: your turn -->
      <template v-else-if="phase === 'Reveal' && isMyTurn">
        <p :class="styles.prompt">Reveal one of your hidden traits:</p>
        <div v-if="myUnrevealedKinds.length > 0" :class="styles.actionRow">
          <button
            v-for="kind in myUnrevealedKinds"
            :key="kind"
            :class="styles.actionButton"
            @click="reveal(kind)"
          >
            {{ kind }}
          </button>
        </div>
        <p v-else :class="styles.status">You have nothing left to reveal.</p>
      </template>

      <!-- Voting: your turn -->
      <template v-else-if="phase === 'Voting' && isMyTurn">
        <p v-if="!hasVoted" :class="styles.prompt">Vote to eliminate someone:</p>
        <p v-else :class="styles.status">Vote cast — waiting for the others.</p>
        <div v-if="!hasVoted && voteable.length > 0" :class="styles.actionRow">
          <button
            v-for="p in voteable"
            :key="p.id"
            :class="styles.actionButton"
            @click="vote(p.id)"
          >
            {{ p.nickname }}
          </button>
        </div>
        <p v-else-if="!hasVoted" :class="styles.status">No one to vote for.</p>
      </template>

      <!-- Roulette: cosmetic spinner -->
      <RouletteSpinner
        v-else-if="phase === 'Roulette'"
        :tied-participant-ids="gameStore.rouletteTiedIds"
        :name-of="nameOf"
      />

      <!-- Discussion phases: chat is the action -->
      <p v-else-if="phase === 'Discussion' || phase === 'DiscussionClosing'" :class="styles.status">
        Chat is open — make your case in the game chat.
      </p>

      <!-- Bunker briefing -->
      <p v-else-if="phase === 'BunkerIntroduction'" :class="styles.status">
        The bunker scenario is being revealed…
      </p>

      <!-- Intro / not your turn in a turn-based phase -->
      <p v-else-if="phase === 'IntroDiscussion' && isMyTurn" :class="styles.status">
        Your turn to introduce yourself — use the game chat.
      </p>

      <p v-else-if="phase === 'Finished'" :class="styles.status">
        The game is over.
      </p>

      <!-- Fallback: waiting for another player's turn -->
      <p v-else :class="styles.status">
        Waiting for {{ currentTurnParticipant?.nickname ?? 'the next player' }}…
      </p>

      <!-- Full tally from the last elimination (revealed at elimination time) -->
      <VoteTally :tally="gameStore.lastTally" :name-of="nameOf" />
    </div>
  </section>
</template>