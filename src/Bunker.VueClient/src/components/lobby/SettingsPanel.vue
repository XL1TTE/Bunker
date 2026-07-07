<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useLobbyStore } from '@/stores/lobby.store';
import { useCatalogStore } from '@/stores/catalog.store';
import { useToast } from '@/composables/useToast';
import styles from '@/components/lobby/settings-panel.module.css';

const lobbyStore = useLobbyStore();
const catalogStore = useCatalogStore();
const { run, success } = useToast();

const lobby = computed(() => lobbyStore.currentLobby);

const localCapacity = ref(lobby.value?.capacity ?? 8);
const localIsPublic = ref(lobby.value?.isPublic ?? true);
const localPacks = ref<string[]>(lobby.value?.selectedPackIds ?? []);

const addingBot = ref(false);
const selectedPresetId = ref<string | null>(null);
const botNickname = ref('');

watch(lobby, (l) => {
  if (!l) return;
  localCapacity.value = l.capacity;
  localIsPublic.value = l.isPublic;
  localPacks.value = [...l.selectedPackIds];
});

function togglePack(id: string): void {
  if (localPacks.value.includes(id)) {
    localPacks.value = localPacks.value.filter((p) => p !== id);
  } else {
    localPacks.value = [...localPacks.value, id];
  }
}

async function saveSettings(): Promise<void> {
  if (!lobby.value) return;
  const ok = await run(() =>
    lobbyStore.updateSettings({
      capacity: localCapacity.value,
      isPublic: localIsPublic.value,
      selectedPackIds: localPacks.value,
    }),
  );
  if (ok !== null) success('Settings saved.');
}

async function addBot(): Promise<void> {
  const presetId = selectedPresetId.value;
  const nickname = botNickname.value.trim();
  if (!presetId || !nickname) return;
  const ok = await run(() =>
    lobbyStore.addBot({
      personalityPresetId: presetId,
      nickname,
    }),
  );
  if (ok === null) return;
  botNickname.value = '';
  selectedPresetId.value = null;
  addingBot.value = false;
}

async function startGame(): Promise<void> {
  await run(() => lobbyStore.start());
}

// Start requirements, aligned with the backend Lobby.StartGame:
//   1. at least 2 human players (bots don't count)
//   2. every non-host human player is Ready (bots are always Ready)
// The host owns this button, so the "must be host" rule is implicit.
const humanPlayers = computed(() =>
  lobby.value ? lobby.value.participants.filter((p) => p.type === 'Player') : [],
);
const nonHostHumans = computed(() => {
  const l = lobby.value;
  return l ? humanPlayers.value.filter((p) => p.id !== l.hostParticipantId) : [];
});
const readyNonHostHumans = computed(() =>
  nonHostHumans.value.filter((p) => p.status === 'Ready'),
);

const playersReqMet = computed(() => humanPlayers.value.length >= 2);
// Vacuously true when there are no other players yet — matches the backend's
// AllReady (All over an empty set). The players requirement is the real blocker then.
const readyReqMet = computed(() => readyNonHostHumans.value.length === nonHostHumans.value.length);

const canStart = computed(() => playersReqMet.value && readyReqMet.value);
</script>

<template>
  <section :class="styles.panel">
    <div :class="styles.panelHeader">
      <h2 :class="styles.panelTitle">Host controls</h2>
    </div>
    <div :class="styles.panelBody">
      <div :class="styles.section">
        <h3 :class="styles.sectionTitle">Lobby settings</h3>
        <label :class="styles.field">
          <span :class="styles.fieldLabel">Capacity</span>
          <input v-model.number="localCapacity" type="number" min="4" max="20" />
        </label>
        <label :class="styles.checkboxField">
          <input v-model="localIsPublic" type="checkbox" />
          <span :class="styles.checkboxFieldBody">
            <span :class="styles.checkboxFieldTitle">Public lobby</span>
            <span :class="styles.checkboxFieldDesc">Visible in the public browser.</span>
          </span>
        </label>
        <div :class="styles.field">
          <span :class="styles.fieldLabel">Card packs</span>
          <div :class="styles.packFieldset">
            <label v-for="p in catalogStore.packs" :key="p.id" :class="styles.packRow">
              <input
                type="checkbox"
                :checked="localPacks.includes(p.id)"
                @change="togglePack(p.id)"
              />
              <span :class="styles.packRowLabel">
                <span :class="styles.packRowTitle">{{ p.title }}</span>
                <span :class="styles.packRowMeta">{{ p.description }}</span>
              </span>
            </label>
          </div>
        </div>
        <button :class="styles.fullButton" @click="saveSettings">Save settings</button>
      </div>

      <hr :class="styles.divider" />

      <div :class="styles.section">
        <h3 :class="styles.sectionTitle">Bots</h3>
        <div :class="styles.botSection">
          <button
            v-if="!addingBot"
            :class="styles.addBotButton"
            @click="addingBot = true"
          >
            + Add bot
          </button>
          <form v-else :class="styles.botForm" @submit.prevent="addBot">
            <label :class="styles.field">
              <span :class="styles.fieldLabel">Personality preset</span>
              <select v-model="selectedPresetId">
                <option :value="null" disabled>Select preset…</option>
                <option v-for="p in catalogStore.presets" :key="p.id" :value="p.id">
                  {{ p.title }}
                </option>
              </select>
            </label>
            <label :class="styles.field">
              <span :class="styles.fieldLabel">Bot nickname</span>
              <input
                v-model="botNickname"
                placeholder="Bot nickname"
                maxlength="20"
              />
            </label>
            <div :class="styles.botFormActions">
              <button type="submit" :class="styles.saveButton">Add bot</button>
              <button type="button" :class="styles.cancelButton" @click="addingBot = false">Cancel</button>
            </div>
          </form>
        </div>
      </div>

      <hr :class="styles.divider" />

      <div :class="styles.section">
        <h3 :class="styles.sectionTitle">Ready to start</h3>
        <ul :class="styles.requirements">
          <li :class="styles.requirement">
            <span :class="[styles.requirementIcon, playersReqMet ? styles.requirementIconMet : '']">
              {{ playersReqMet ? '✓' : '○' }}
            </span>
            <span :class="styles.requirementText">
              At least 2 players
              <span :class="styles.requirementMeta">{{ humanPlayers.length }}/2</span>
            </span>
          </li>
          <li :class="styles.requirement">
            <span :class="[styles.requirementIcon, readyReqMet ? styles.requirementIconMet : '']">
              {{ readyReqMet ? '✓' : '○' }}
            </span>
            <span :class="styles.requirementText">
              All other players ready
              <span :class="styles.requirementMeta">
                {{ readyNonHostHumans.length }}/{{ nonHostHumans.length }}
              </span>
            </span>
          </li>
        </ul>
      </div>

      <button
        :class="styles.startButton"
        :disabled="!canStart"
        :title="canStart ? 'Start the game' : 'Fulfill the requirements above to start'"
        @click="startGame"
      >
        Start game
      </button>
    </div>
  </section>
</template>