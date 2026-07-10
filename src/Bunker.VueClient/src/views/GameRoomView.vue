<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useGameStore } from '@/stores/game.store';
import { useToast } from '@/composables/useToast';
import type { GamePhase } from '@/types/game.types';
import BunkerCardPanel from '@/components/game/BunkerCardPanel.vue';
import CharacterSheet from '@/components/game/CharacterSheet.vue';
import ActionPanel from '@/components/game/ActionPanel.vue';
import ChatPanel from '@/components/game/ChatPanel.vue';
import styles from '@/views/game-room.module.css';

const props = defineProps<{ gameId: string }>();
const router = useRouter();
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

const game = computed(() => gameStore.currentGame);
const phaseLabel = computed(() => (gameStore.phase ? PHASE_LABELS[gameStore.phase] : '—'));
const aliveCount = computed(() => gameStore.aliveCount);
const survivorNames = computed(() =>
  gameStore.survivors.map((id) => {
    const p = gameStore.participants.find((x) => x.id === id);
    return p?.nickname ?? id.slice(0, 8);
  }),
);

function isCurrentTurn(participantId: string): boolean {
  return gameStore.currentTurnParticipantId === participantId;
}

onMounted(async () => {
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
  gameStore.disconnectRealtime();
});

async function backToLobbies(): Promise<void> {
  await router.push('/lobbies');
}
</script>

<template>
  <section :class="styles.container">
    <header :class="styles.header">
      <div :class="styles.headerLeft">
        <h1 :class="styles.title">Bunker</h1>
        <p :class="styles.subline">
          <span :class="styles.metaItem">Round {{ game?.roundNumber ?? '—' }}</span>
          <span :class="styles.metaSep" aria-hidden="true"></span>
          <span :class="styles.metaItem">{{ phaseLabel }}</span>
          <span :class="styles.metaSep" aria-hidden="true"></span>
          <span :class="styles.metaItem">{{ aliveCount }} / {{ game?.bunkerCapacity ?? '—' }} to survive</span>
        </p>
      </div>
      <div :class="styles.headerRight">
        <button :class="styles.leaveButton" @click="backToLobbies">Leave game</button>
      </div>
    </header>

    <div :class="styles.grid">
      <div :class="styles.mainColumn">
        <BunkerCardPanel :bunker-card="gameStore.bunkerCard" />
        <ActionPanel />
        <section :class="styles.panel">
          <div :class="styles.panelHeader">
            <h2 :class="styles.panelTitle">Participants</h2>
            <span :class="styles.panelMeta">{{ aliveCount }} alive</span>
          </div>
          <div :class="styles.panelBody">
            <ul :class="styles.sheetList">
              <li v-for="p in gameStore.participants" :key="p.id">
                <CharacterSheet :participant="p" :is-current-turn="isCurrentTurn(p.id)" />
              </li>
            </ul>
          </div>
        </section>
      </div>

      <div :class="styles.sideColumn">
        <ChatPanel />
      </div>
    </div>

    <Teleport to="body">
      <div v-if="gameStore.finished" :class="styles.modalBackdrop" @click.self="backToLobbies">
        <div :class="styles.modal" role="dialog" aria-modal="true">
          <h2>Game over</h2>
          <p>These players made it into the bunker:</p>
          <ul :class="styles.survivorList">
            <li v-for="name in survivorNames" :key="name" :class="styles.survivorItem">{{ name }}</li>
          </ul>
          <div :class="styles.modalActions">
            <button :class="styles.modalButton" @click="backToLobbies">Back to lobbies</button>
          </div>
        </div>
      </div>
    </Teleport>
  </section>
</template>