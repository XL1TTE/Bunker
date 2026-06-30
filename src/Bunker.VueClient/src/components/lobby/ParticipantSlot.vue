<script setup lang="ts">
import { computed } from 'vue';
import type { Participant } from '@/types/lobby.types';
import styles from '@/components/lobby/participant-slot.module.css';

const props = defineProps<{
  participant: Participant;
  isHost: boolean;
  selfAccountId: string | undefined;
  hostParticipantId: string;
}>();

const emit = defineEmits<{
  kick: [participantId: string];
  removeBot: [participantId: string];
}>();

const isSelf = computed(() => props.participant.accountId === props.selfAccountId);
const isHostSlot = computed(() => props.participant.id === props.hostParticipantId);
const isBot = computed(() => props.participant.type === 'Bot');

const initial = computed(() =>
  props.participant.nickname?.[0]?.toUpperCase() ?? '?',
);

const avatarClass = computed(() => {
  if (isHostSlot.value) return styles.avatarHost;
  if (isBot.value) return styles.avatarBot;
  return styles.avatarPlayer;
});

const readyLabel = computed(() => props.participant.status);

function onKick(): void {
  emit('kick', props.participant.id);
}

function onRemoveBot(): void {
  emit('removeBot', props.participant.id);
}
</script>

<template>
  <article
    :class="[
      styles.slot,
      isHostSlot ? styles.hostSlot : '',
      isBot ? styles.botSlot : '',
    ]"
  >
    <span :class="[styles.avatar, avatarClass]" aria-hidden="true">
      {{ initial }}
    </span>
    <div :class="styles.info">
      <div :class="styles.nicknameRow">
        <span :class="styles.nickname">{{ participant.nickname }}</span>
        <span v-if="isSelf" :class="styles.youBadge">You</span>
      </div>
      <div :class="styles.tags">
        <span v-if="isBot" :class="styles.botBadge">Bot</span>
        <span v-if="isHostSlot" :class="styles.hostBadge">Host</span>
        <span
          :class="[
            styles.statusBadge,
            participant.status === 'Ready' ? styles.statusReady : styles.statusNotReady,
          ]"
        >
          {{ readyLabel === 'Ready' ? '● Ready' : '○ Not ready' }}
        </span>
      </div>
    </div>
    <div :class="styles.actions">
      <button
        v-if="isHost && !isSelf && !isBot"
        :class="styles.kickButton"
        @click="onKick"
        :title="`Kick ${participant.nickname}`"
      >
        Kick
      </button>
      <button
        v-if="isHost && isBot"
        :class="styles.kickButton"
        @click="onRemoveBot"
        :title="`Remove bot ${participant.nickname}`"
      >
        Remove
      </button>
    </div>
  </article>
</template>