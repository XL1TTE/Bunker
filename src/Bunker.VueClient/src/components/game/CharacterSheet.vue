<script setup lang="ts">
import { computed } from 'vue';
import type { ParticipantDto } from '@/types/game.types';
import styles from '@/components/game/character-sheet.module.css';

const props = defineProps<{ participant: ParticipantDto; isCurrentTurn: boolean }>();

// A friendly label for each attribute kind. The backend sends the canonical kind
// string (Profession, Hobbies, Age, Sex, Fact, Health, Luggage).
const KIND_LABELS: Record<string, string> = {
  Profession: 'Profession',
  Hobbies: 'Hobbies',
  Age: 'Age',
  Sex: 'Sex',
  Fact: 'Fact',
  Health: 'Health',
  Luggage: 'Luggage',
};

function labelFor(kind: string): string {
  return KIND_LABELS[kind] ?? kind;
}

const revealedCount = computed(() => props.participant.attributes.filter((a) => a.revealed).length);
</script>

<template>
  <article
    :class="[
      styles.sheet,
      participant.eliminated ? styles.eliminated : '',
      isCurrentTurn ? styles.currentTurn : '',
      participant.isYou ? styles.self : '',
    ]"
  >
    <header :class="styles.header">
      <div :class="styles.identity">
        <span :class="styles.nickname">{{ participant.nickname }}</span>
        <span v-if="participant.isYou" :class="styles.youTag">You</span>
        <span :class="[styles.typeTag, participant.type === 'Bot' ? styles.typeBot : styles.typePlayer]">
          {{ participant.type === 'Bot' ? 'Bot' : 'Player' }}
        </span>
        <span v-if="participant.eliminated" :class="styles.eliminatedTag">Eliminated</span>
      </div>
      <span v-if="isCurrentTurn && !participant.eliminated" :class="styles.turnTag">Their turn</span>
    </header>

    <ul :class="styles.attrList">
      <li v-for="attr in participant.attributes" :key="attr.kind" :class="styles.attrItem">
        <span :class="styles.attrKind">{{ labelFor(attr.kind) }}</span>
        <span :class="styles.attrValue">{{ attr.value }}</span>
        <span
          v-if="participant.isYou"
          :class="[styles.attrBadge, attr.revealed ? styles.attrPublic : styles.attrPrivate]"
        >
          {{ attr.revealed ? 'Public' : 'Private' }}
        </span>
      </li>
    </ul>

    <p v-if="participant.attributes.length === 0" :class="styles.empty">
      No traits revealed yet.
    </p>
    <p v-else-if="!participant.isYou" :class="styles.hint">
      {{ revealedCount }} trait{{ revealedCount === 1 ? '' : 's' }} revealed.
    </p>
  </article>
</template>