<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useLobbyStore } from '@/stores/lobby.store';
import { useToast } from '@/composables/useToast';
import styles from '@/views/lobby-create.module.css';

const router = useRouter();
const lobbyStore = useLobbyStore();
const { run } = useToast();

const MIN_CAPACITY = 4;
const MAX_CAPACITY = 20;

const name = ref('');
const capacity = ref(8);
const isPublic = ref(true);
const password = ref('');
const submitting = ref(false);

const trimmedName = computed(() => name.value.trim());
const canCreate = computed(() => trimmedName.value.length > 0 && trimmedName.value.length <= 60);

const canDecrement = computed(() => capacity.value > MIN_CAPACITY);
const canIncrement = computed(() => capacity.value < MAX_CAPACITY);

function decrement(): void {
  if (canDecrement.value) capacity.value -= 1;
}
function increment(): void {
  if (canIncrement.value) capacity.value += 1;
}

// The field is typeable, so enforce the range on blur (the +/- buttons clamp on
// their own via the disabled bounds). An empty/invalid entry snaps back to the
// minimum rather than sending a NaN to the backend.
function clampCapacity(): void {
  const n = capacity.value;
  if (!Number.isFinite(n)) {
    capacity.value = MIN_CAPACITY;
    return;
  }
  capacity.value = Math.min(MAX_CAPACITY, Math.max(MIN_CAPACITY, Math.round(n)));
}

async function createNew(): Promise<void> {
  // The name is the lobby's identity in the browser list, so it's required.
  // Trim once at submit rather than mutating the field the user is typing in.
  const lobbyName = trimmedName.value;
  if (!lobbyName) return;
  submitting.value = true;
  try {
    // Passwords only apply to public lobbies (private lobbies are invite-code
    // only). Send undefined when empty so the backend stores no password.
    const lobby = await run(() =>
      lobbyStore.create({
        name: lobbyName,
        capacity: capacity.value,
        isPublic: isPublic.value,
        selectedPackIds: [],
        password: isPublic.value && password.value ? password.value : undefined,
      }),
    );
    if (lobby) {
      await router.push({ name: 'lobby-room', params: { id: lobby.id } });
    }
  } finally {
    submitting.value = false;
  }
}
</script>

<template>
  <section :class="styles.page">
    <header :class="styles.pageHead">
      <span :class="styles.eyebrow">Host</span>
      <h1 :class="styles.title">Create a lobby</h1>
      <p :class="styles.subtitle">Set up your game. You can change these settings later.</p>
    </header>

    <div :class="styles.card">
      <form :class="styles.form" @submit.prevent="createNew">
        <!-- Lobby name — the title others see in the browser list. Required,
             since the list shows names (invite codes are private to the host). -->
        <div :class="styles.group">
          <div :class="styles.groupHead">
            <span :class="styles.label">Lobby name</span>
            <span :class="styles.hint">
              This is how your lobby shows up in the browser. Keep it short and recognizable.
            </span>
          </div>
          <input
            v-model="name"
            type="text"
            :class="styles.input"
            placeholder="Friday Night Bunker"
            maxlength="60"
            autocomplete="off"
          />
        </div>

        <!-- Capacity — a typeable number field with custom +/- buttons to the
             right (the native spinner arrows are hidden; these replace them). -->
        <div :class="styles.group">
          <div :class="styles.groupHead">
            <span :class="styles.label">Capacity</span>
            <span :class="styles.hint">
              How many players and bots the bunker can hold ({{ MIN_CAPACITY }}–{{ MAX_CAPACITY }}).
            </span>
          </div>
          <div :class="styles.capacityControl">
            <input
              v-model.number="capacity"
              type="number"
              :min="MIN_CAPACITY"
              :max="MAX_CAPACITY"
              :class="styles.capacityInput"
              @blur="clampCapacity"
            />
            <div :class="styles.spinBtns">
              <button
                type="button"
                :class="styles.spinBtn"
                :disabled="!canDecrement"
                aria-label="Decrease capacity"
                @click="decrement"
              >
                −
              </button>
              <button
                type="button"
                :class="styles.spinBtn"
                :disabled="!canIncrement"
                aria-label="Increase capacity"
                @click="increment"
              >
                +
              </button>
            </div>
          </div>
        </div>

        <!-- Visibility — a segmented Public/Private toggle. The hint explains
             the tradeoff of the current choice, not both. -->
        <div :class="styles.group">
          <div :class="styles.groupHead">
            <span :class="styles.label">Visibility</span>
            <span :class="styles.hint">
              {{ isPublic
                ? 'Anyone browsing can join.'
                : 'Only people with the invite code can join.' }}
            </span>
          </div>
          <div :class="styles.segmented" role="group" aria-label="Lobby visibility">
            <button
              type="button"
              :class="[styles.segBtn, isPublic && styles.segBtnActive]"
              @click="isPublic = true"
            >
              Public
            </button>
            <button
              type="button"
              :class="[styles.segBtn, !isPublic && styles.segBtnActive]"
              @click="isPublic = false"
            >
              Private
            </button>
          </div>
        </div>

        <!-- Password — only meaningful for a public lobby (private lobbies are
             already invite-only). -->
        <div v-if="isPublic" :class="styles.group">
          <div :class="styles.groupHead">
            <span :class="styles.label">Password (optional)</span>
            <span :class="styles.hint">Players joining from the browser will need it.</span>
          </div>
          <input
            v-model="password"
            type="password"
            :class="styles.input"
            placeholder="Leave empty for open access"
            autocomplete="new-password"
            maxlength="100"
          />
          <span :class="styles.note">
            Anyone with the invite code can still join without it.
          </span>
        </div>

        <button type="submit" :class="styles.submit" :disabled="submitting || !canCreate">
          {{ submitting ? 'Creating…' : 'Create lobby' }}
        </button>
      </form>
    </div>
  </section>
</template>