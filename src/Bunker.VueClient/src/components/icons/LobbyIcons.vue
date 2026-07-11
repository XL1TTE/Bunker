<script setup lang="ts">
// Icon set for the lobbies browser/create flow. Single <svg> root so the
// parent `class` falls through. Shares the lucide stroke language
// (currentColor, stroke-width 2, rounded caps) with FeatureIcon / AiCardIcon
// etc. so the whole icon set reads as one system.
type LobbyIconName = 'plus' | 'key' | 'lock' | 'users' | 'sparkle';

defineProps<{ name: LobbyIconName }>();
</script>

<template>
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
    <!-- New lobby / host action -->
    <template v-if="name === 'plus'">
      <path d="M12 5v14" />
      <path d="M5 12h14" />
    </template>

    <!-- Join with a code: a key -->
    <template v-else-if="name === 'key'">
      <circle cx="7.5" cy="15.5" r="4.5" />
      <path d="M10.5 12.5 20 3" />
      <path d="M16 7l3 3" />
      <path d="M18.5 4.5 21 7" />
    </template>

    <!-- Password-protected lobby: a padlock (replaces the 🔒 emoji) -->
    <template v-else-if="name === 'lock'">
      <rect x="4" y="11" width="16" height="10" rx="1.5" />
      <path d="M8 11V7a4 4 0 0 1 8 0v4" />
    </template>

    <!-- Capacity / public lobbies: a group -->
    <template v-else-if="name === 'users'">
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </template>

    <!-- Empty state: a sparkle -->
    <template v-else-if="name === 'sparkle'">
      <path d="M12 3l1.8 5.2L19 10l-5.2 1.8L12 17l-1.8-5.2L5 10l5.2-1.8z" />
      <path d="M19 15l.7 2.3L22 18l-2.3.7L19 21l-.7-2.3L16 18l2.3-.7z" />
    </template>
  </svg>
</template>