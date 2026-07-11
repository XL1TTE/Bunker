<script setup lang="ts">
// One icon set for the "What You Get" feature cards. Single <svg> root so the
// parent `class` falls through. All icons share the lucide stroke language
// (currentColor, stroke-width 2, rounded caps) to read as one system.
type FeatureName = 'realtime' | 'bots' | 'packs' | 'stats' | 'lobbies' | 'durability';

defineProps<{ name: FeatureName }>();
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
    <!-- Real-time multiplayer: activity pulse -->
    <template v-if="name === 'realtime'">
      <path d="M22 12h-4l-3 9L9 3l-3 9H2" />
    </template>

    <!-- AI bots: bot head -->
    <template v-else-if="name === 'bots'">
      <rect x="3" y="8" width="18" height="12" rx="2" />
      <path d="M12 4v4" />
      <circle cx="12" cy="2" r="1" />
      <circle cx="9" cy="14" r="1.4" />
      <circle cx="15" cy="14" r="1.4" />
    </template>

    <!-- Card packs: stacked layers -->
    <template v-else-if="name === 'packs'">
      <path d="M12 2 22 8.5 12 15 2 8.5 12 2z" />
      <path d="M2 15.5 12 22 22 15.5" />
    </template>

    <!-- Profiles and stats: bar chart -->
    <template v-else-if="name === 'stats'">
      <line x1="6" y1="20" x2="6" y2="14" />
      <line x1="12" y1="20" x2="12" y2="8" />
      <line x1="18" y1="20" x2="18" y2="4" />
      <line x1="3" y1="20" x2="21" y2="20" />
    </template>

    <!-- Private/public lobbies: users -->
    <template v-else-if="name === 'lobbies'">
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </template>

    <!-- Survives interruptions: shield -->
    <template v-else-if="name === 'durability'">
      <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
    </template>
  </svg>
</template>