<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useLobbyStore } from '@/stores/lobby.store';
import { useToast } from '@/composables/useToast';
import styles from '@/views/lobby-create.module.css';

const router = useRouter();
const lobbyStore = useLobbyStore();
const { run } = useToast();

const capacity = ref(8);
const isPublic = ref(true);
const password = ref('');
const inviteCode = ref('');
const submitting = ref(false);

async function createNew(): Promise<void> {
  submitting.value = true;
  try {
    // Passwords only apply to public lobbies (private lobbies are invite-code
    // only). Send undefined when empty so the backend stores no password.
    const lobby = await run(() =>
      lobbyStore.create({
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

async function joinExisting(): Promise<void> {
  if (!inviteCode.value.trim()) return;
  submitting.value = true;
  try {
    const lobby = await run(() => lobbyStore.joinByCode(inviteCode.value.trim().toUpperCase()));
    if (lobby) {
      await router.push({ name: 'lobby-room', params: { id: lobby.id } });
    }
  } finally {
    submitting.value = false;
  }
}
</script>

<template>
  <section :class="styles.container">
    <div :class="styles.heading">
      <h1>Enter a lobby</h1>
      <p>Have an invite code? Join straight in. Otherwise, host your own game.</p>
    </div>

    <div :class="styles.card">
      <h2>Join by code</h2>
      <p :class="styles.cardHint">
        Got a code from a friend? Pop it in and you're in.
      </p>
      <form :class="styles.form" @submit.prevent="joinExisting">
        <label :class="styles.field">
          <span :class="styles.fieldLabel">Invite code</span>
          <input
            v-model="inviteCode"
            :class="styles.input"
            placeholder="ABC123"
            maxlength="16"
            spellcheck="false"
            autocapitalize="characters"
          />
        </label>
        <div :class="styles.formActions">
          <button
            type="submit"
            :disabled="submitting || !inviteCode"
            :class="styles.button"
          >
            Join lobby
          </button>
        </div>
      </form>
    </div>

    <div :class="styles.card">
      <h2>Create new</h2>
      <p :class="styles.cardHint">
        Configure your game. You can change these settings later.
      </p>
      <form :class="styles.form" @submit.prevent="createNew">
        <label :class="styles.field">
          <span :class="styles.fieldLabel">Capacity</span>
          <input
            v-model.number="capacity"
            type="number"
            min="4"
            max="20"
            :class="styles.input"
          />
          <span :class="styles.fieldHint">How many players (and bots) the bunker can hold.</span>
        </label>

        <label :class="styles.checkboxField">
          <input v-model="isPublic" type="checkbox" />
          <span :class="styles.checkboxFieldBody">
            <span :class="styles.checkboxFieldTitle">Public lobby</span>
            <span :class="styles.checkboxFieldDesc">
              Visible in the public browser. Turn off to keep it invite-only.
            </span>
          </span>
        </label>

        <label v-if="isPublic" :class="styles.field">
          <span :class="styles.fieldLabel">Password (optional)</span>
          <input
            v-model="password"
            type="password"
            :class="styles.input"
            placeholder="Leave empty for open access"
            autocomplete="new-password"
            maxlength="100"
          />
          <span :class="styles.fieldHint">
            Players joining from the browser will need this. Anyone with the
            invite code can still join without it.
          </span>
        </label>

        <div :class="styles.formActions">
          <button type="submit" :disabled="submitting" :class="styles.button">
            Create lobby
          </button>
        </div>
      </form>
    </div>
  </section>
</template>