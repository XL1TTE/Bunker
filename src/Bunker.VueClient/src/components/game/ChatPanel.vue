<script setup lang="ts">
import { computed, nextTick, ref, watch } from 'vue';
import { useGameStore } from '@/stores/game.store';
import { useToast } from '@/composables/useToast';
import styles from '@/components/game/chat-panel.module.css';

const gameStore = useGameStore();
const { run } = useToast();
const draft = ref('');
const scrollEl = ref<HTMLElement | null>(null);

defineEmits<{ collapse: [] }>();

const messages = computed(() => gameStore.messages);
const canSend = computed(
  () => !!gameStore.currentGame && !gameStore.finished && draft.value.trim().length > 0,
);

function timeOf(sentAt: string): string {
  const d = new Date(sentAt);
  return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

async function send(): Promise<void> {
  const text = draft.value.trim();
  if (!text || !gameStore.currentGame) return;
  const ok = await run(() => gameStore.sendMessage(text));
  if (ok === null) return; // keep the draft so the user can retry
  draft.value = '';
  await nextTick();
  if (scrollEl.value) {
    scrollEl.value.scrollTop = scrollEl.value.scrollHeight;
  }
}

watch(
  () => messages.value.length,
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
      <h2 :class="styles.panelTitle">Game chat</h2>
      <div :class="styles.panelMeta">
        <span :class="styles.liveDot" aria-hidden="true"></span>
        <span>Live</span>
        <button type="button" :class="styles.collapseBtn" aria-label="Hide chat" @click="$emit('collapse')">
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
            <polyline points="9 18 15 12 9 6" />
          </svg>
        </button>
      </div>
    </div>
    <div ref="scrollEl" :class="styles.scroll">
      <p v-if="messages.length === 0" :class="styles.empty">
        No messages yet — discuss who should stay in the bunker.
      </p>
      <article v-for="m in messages" :key="m.id" :class="styles.message">
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
        :disabled="!gameStore.currentGame || gameStore.finished"
      />
      <button type="submit" :class="styles.sendButton" :disabled="!canSend">Send</button>
    </form>
  </section>
</template>