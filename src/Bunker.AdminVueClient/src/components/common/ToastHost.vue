<script setup lang="ts">
import { useToastStore } from '@/stores/toast.store';
import styles from '@/components/common/toast-host.module.css';

const toastStore = useToastStore();

const iconFor: Record<string, string> = {
  error: '!',
  success: '✓',
  info: 'i',
};
</script>

<template>
  <Teleport to="body">
    <div :class="styles.host" role="status" aria-live="polite">
      <transition-group
        :enter-active-class="styles.enterActive"
        :leave-active-class="styles.leaveActive"
        :enter-from-class="styles.enterFrom"
        :leave-to-class="styles.leaveTo"
        :move-class="styles.move"
      >
        <div
          v-for="t in toastStore.toasts"
          :key="t.id"
          :class="[styles.toast, styles[t.type]]"
        >
          <span :class="styles.icon" aria-hidden="true">{{ iconFor[t.type] }}</span>
          <span :class="styles.message">{{ t.message }}</span>
          <button :class="styles.close" :aria-label="`Dismiss ${t.type} notification`" @click="toastStore.dismiss(t.id)">×</button>
        </div>
      </transition-group>
    </div>
  </Teleport>
</template>