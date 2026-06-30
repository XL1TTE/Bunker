<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useLobbyStore } from '@/stores/lobby.store';
import LobbySummaryCard from '@/components/lobby/LobbySummaryCard.vue';
import styles from '@/views/lobby-browser.module.css';

const router = useRouter();
const lobbyStore = useLobbyStore();

onMounted(async () => {
  await lobbyStore.fetchBrowser();
});

async function openLobby(id: string): Promise<void> {
  await router.push({ name: 'lobby-room', params: { id } });
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
        <LobbySummaryCard :summary="summary" @join="openLobby(summary.id)" />
      </li>
    </ul>
  </section>
</template>