<script setup lang="ts">
import { computed } from 'vue';
import type { ParticipantDto } from '@/types/game.types';
import LockIcon from '@/components/icons/LockIcon.vue';
import styles from '@/components/game/character-sheet.module.css';

const props = defineProps<{ participant: ParticipantDto; isCurrentTurn: boolean }>();

const isBot = computed(() => props.participant.type === 'Bot');
const initial = computed(() => props.participant.nickname?.[0]?.toUpperCase() ?? '?');
const isSelf = computed(() => props.participant.isYou);

// Attributes live ON the webcam tile as colored tags (no separate section
// below, so every card stays the same height and the grid stays tight). The
// identity core — Profession, Age, Health — clusters top-left in a horizontal
// row; the rest cluster bottom-right in a right-aligned vertical stack. Each
// kind gets its own tag color (see the .kind* classes in the stylesheet) so a
// glance reads the kind without a label.
const TOP_LEFT_KINDS: readonly string[] = ['Profession', 'Age', 'Health'];
const topLeft = computed(() =>
  props.participant.attributes.filter((a) => TOP_LEFT_KINDS.includes(a.kind)),
);
const bottomRight = computed(() =>
  props.participant.attributes.filter((a) => !TOP_LEFT_KINDS.includes(a.kind)),
);

const KIND_CLASS: Record<string, string> = {
  Profession: styles.kindProfession,
  Age: styles.kindAge,
  Health: styles.kindHealth,
  Hobbies: styles.kindHobbies,
  Sex: styles.kindSex,
  Fact: styles.kindFact,
  Luggage: styles.kindLuggage,
};
function kindClass(kind: string): string {
  return KIND_CLASS[kind] ?? styles.kindProfession;
}

// Another player's unrevealed attribute is private to them — we deliberately
// do NOT render its value. It still takes a tag (in its kind color, dimmed)
// with a lock + "Hidden", so you can see WHICH trait is hidden at a glance.
// For yourself every value is visible; a not-yet-revealed (private) one gets a
// lock so you know it isn't public yet.
function isLocked(attr: { revealed: boolean }): boolean {
  return !isSelf.value && !attr.revealed;
}
function isPrivate(attr: { revealed: boolean }): boolean {
  return isSelf.value && !attr.revealed;
}
</script>

<template>
  <article
    :class="[
      styles.sheet,
      isCurrentTurn ? styles.currentTurn : '',
      isSelf ? styles.self : '',
      participant.eliminated ? styles.eliminated : '',
    ]"
  >
    <!-- Webcam-ready tile. A 16:9 rectangle that today shows the player's
         gradient initial; the inner .videoLayer is where a future <video>
         webcam feed drops in (no webcam logic yet — TODO(webcam)). The
         attributes, name, and identity tags all overlay the tile so the
         rectangle stays the focal point and the card keeps a fixed height. -->
    <div :class="styles.tile">
      <div :class="[styles.videoLayer, isBot ? styles.tileBot : styles.tilePlayer]" aria-hidden="true">
        <span :class="styles.initial">{{ initial }}</span>
      </div>
      <!-- TODO(webcam): a <video> element bound to this participant's stream
           would slot in here, layered above .videoLayer. -->

      <!-- Identity core: Profession / Age / Health — top-left, horizontal. The
           right side is left clear for the "Their turn" badge. -->
      <div :class="styles.attrTopLeft">
        <span
          v-for="attr in topLeft"
          :key="attr.kind"
          :class="[
            styles.attrTag,
            kindClass(attr.kind),
            isLocked(attr) ? styles.attrLocked : '',
            isPrivate(attr) ? styles.attrPrivate : '',
          ]"
        >
          <LockIcon v-if="isLocked(attr) || isPrivate(attr)" :class="styles.tagLock" />
          <span v-if="isLocked(attr)" :class="styles.tagHidden">Hidden</span>
          <span v-else :class="styles.tagValue">{{ attr.value }}</span>
        </span>
      </div>

      <!-- The rest: Hobbies / Sex / Fact / Luggage — bottom-right, vertical,
           right-aligned. -->
      <div :class="styles.attrBottomRight">
        <span
          v-for="attr in bottomRight"
          :key="attr.kind"
          :class="[
            styles.attrTag,
            kindClass(attr.kind),
            isLocked(attr) ? styles.attrLocked : '',
            isPrivate(attr) ? styles.attrPrivate : '',
          ]"
        >
          <LockIcon v-if="isLocked(attr) || isPrivate(attr)" :class="styles.tagLock" />
          <span v-if="isLocked(attr)" :class="styles.tagHidden">Hidden</span>
          <span v-else :class="styles.tagValue">{{ attr.value }}</span>
        </span>
      </div>

      <!-- Name + identity tags over the bottom scrim, kept to the left so the
           bottom-right corner is free for the attribute stack. -->
      <div :class="styles.tileOverlay">
        <span :class="styles.nickname">{{ participant.nickname }}</span>
        <span v-if="isSelf" :class="styles.youTag">You</span>
        <span :class="[styles.typeTag, isBot ? styles.typeBot : styles.typePlayer]">
          {{ isBot ? 'Bot' : 'Player' }}
        </span>
        <span v-if="participant.eliminated" :class="styles.eliminatedTag">Out</span>
      </div>

      <span v-if="isCurrentTurn && !participant.eliminated" :class="styles.turnBadge">Their turn</span>

      <p v-if="participant.attributes.length === 0" :class="styles.empty">No traits yet.</p>
    </div>
  </article>
</template>