<script setup lang="ts">
import styles from '@/components/common/modal.module.css';

defineProps<{ open: boolean; title?: string }>();
defineEmits<{ close: [] }>();
</script>

<template>
  <Teleport to="body">
    <div v-if="open" :class="styles.backdrop" @click.self="$emit('close')">
      <div :class="styles.modal">
        <h2 v-if="title" :class="styles.title">{{ title }}</h2>
        <slot />
      </div>
    </div>
  </Teleport>
</template>