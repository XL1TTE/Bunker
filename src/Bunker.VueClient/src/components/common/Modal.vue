<script setup lang="ts">
import { nextTick, ref, watch } from 'vue';
import styles from '@/components/common/modal.module.css';

const props = defineProps<{ open: boolean; title?: string }>();
const emit = defineEmits<{ close: [] }>();

const modalEl = ref<HTMLElement | null>(null);

// Esc closes the modal; the listener is only active while open so a closed
// modal never swallows Esc from the page behind it. Added/removed on the
// window so it works regardless of focus location inside the dialog.
function onKeydown(e: KeyboardEvent): void {
  if (e.key === 'Escape' && props.open) {
    e.preventDefault();
    emit('close');
  }
}

// Move focus into the dialog when it opens (keyboard + screen-reader users
// land on the dialog, not whatever was focused behind the backdrop), and
// restore it when the dialog closes.
let previouslyFocused: HTMLElement | null = null;

watch(
  () => props.open,
  async (isOpen) => {
    if (isOpen) {
      window.addEventListener('keydown', onKeydown);
      previouslyFocused = document.activeElement as HTMLElement | null;
      await nextTick();
      // Focus the dialog shell itself (or the first focusable child if any);
      // the shell is tabIndex=-1 so it's focusable but not in the tab order.
      modalEl.value?.focus();
    } else {
      window.removeEventListener('keydown', onKeydown);
      previouslyFocused?.focus?.();
      previouslyFocused = null;
    }
  },
);
</script>

<template>
  <Teleport to="body">
    <div v-if="open" :class="styles.backdrop" @click.self="emit('close')">
      <div
        ref="modalEl"
        :class="styles.modal"
        role="dialog"
        aria-modal="true"
        :aria-label="title"
        tabindex="-1"
      >
        <button
          type="button"
          :class="styles.close"
          aria-label="Close"
          @click="emit('close')"
        >
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
            <line x1="18" y1="6" x2="6" y2="18" />
            <line x1="6" y1="6" x2="18" y2="18" />
          </svg>
        </button>
        <h2 v-if="title" :class="styles.title">{{ title }}</h2>
        <slot />
      </div>
    </div>
  </Teleport>
</template>