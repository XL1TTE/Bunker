<script setup lang="ts">
import { nextTick, ref, watch } from 'vue';
import { useLobbyStore } from '@/stores/lobby.store';
import styles from '@/components/lobby/chat-panel.module.css';

const lobbyStore = useLobbyStore();
const draft = ref('');
const scrollEl = ref<HTMLElement | null>(null);

function timeOf(sentAt: string): string {
  const d = new Date(sentAt);
  return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

async function send(): Promise<void> {
  const text = draft.value.trim();
  if (!text) return;
  draft.value = '';
  await lobbyStore.sendMessage(text);
  await nextTick();
  if (scrollEl.value) {
    scrollEl.value.scrollTop = scrollEl.value.scrollHeight;
  }
}

watch(
  () => lobbyStore.messages.length,
  async () => {
    await nextTick();
    if (scrollEl.value) {
      scrollEl.value.scrollTop = scrollEl.value.scrollHeight;
    }
  },
);
</script>

<template>
  <section :class="styles.panel">
    <div :class="styles.panelHeader">
      <h2 :class="styles.panelTitle">Lobby chat</h2>
      <span :class="styles.panelMeta">
        <span :class="styles.liveDot" aria-hidden="true"></span>
        Live
      </span>
    </div>
    <div ref="scrollEl" :class="styles.scroll">
      <p v-if="lobbyStore.messages.length === 0" :class="styles.empty">
        No messages yet — say hello.
      </p>
      <article v-for="m in lobbyStore.messages" :key="m.id" :class="styles.message">
        <header :class="styles.messageHeader">
          <span :class="styles.messageNick">{{ m.nickname }}</span>
          <time :class="styles.messageTime">{{ timeOf(m.sentAt) }}</time>
        </header>
        <p :class="styles.messageText">{{ m.text }}</p>
      </article>
    </div>
    <form :class="styles.composer" @submit.prevent="send">
      <input
        v-model="draft"
        :class="styles.input"
        placeholder="Type a message…"
        maxlength="500"
      />
      <button type="submit" :class="styles.sendButton" :disabled="!draft.trim()">Send</button>
    </form>
  </section>
</template>