<script setup lang="ts">
// Icon set for the "Powered by AI" section's three cards. Single <svg> root so
// the parent `class` falls through. Shares the lucide stroke language
// (currentColor, stroke-width 2, rounded caps) with FeatureIcon / SunIcon etc.
// so the whole icon set reads as one system.
type AiCardName = 'shuffle' | 'sheet' | 'complete';

defineProps<{ name: AiCardName }>();
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
    <!-- A Unique Bunker Every Game: shuffle — a fresh draw each match -->
    <template v-if="name === 'shuffle'">
      <path d="M16 3h5v5" />
      <path d="M4 20 21 3" />
      <path d="M21 16v5h-5" />
      <path d="M15 15l6 6" />
      <path d="M4 4l5 5" />
    </template>

    <!-- Coherent Character Sheets: a document with content lines -->
    <template v-else-if="name === 'sheet'">
      <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
      <path d="M14 2v6h6" />
      <path d="M8 13h8" />
      <path d="M8 17h8" />
      <path d="M8 9h3" />
    </template>

    <!-- Always Playable: a completed check — runs to its conclusion -->
    <template v-else-if="name === 'complete'">
      <circle cx="12" cy="12" r="9" />
      <path d="M8.5 12.5l2.5 2.5 4.5-5" />
    </template>
  </svg>
</template>