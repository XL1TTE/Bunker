<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue';
import styles from '@/components/landing/bunker-atmosphere.module.css';

// Ambient dust/embers drifting up, two slow glow blobs, an occasional signal
// flicker, and subtle scroll parallax. transform/opacity only -> 60fps.
const root = ref<HTMLElement | null>(null);
let raf = 0;
let lastY = -1;
let onScroll: (() => void) | null = null;

// ~18 motes; randomized inline custom props drive per-mote variation so a
// single set of keyframes covers them all.
const motes = Array.from({ length: 18 }, (_, i) => ({
  id: i,
  left: Math.random() * 100,
  delay: -Math.random() * 24,
  duration: 18 + Math.random() * 14,
  size: 2 + Math.random() * 3,
  drift: (Math.random() * 2 - 1) * 24,
}));

onMounted(() => {
  const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  if (reduce || !root.value) return;

  onScroll = () => {
    if (raf) return;
    raf = requestAnimationFrame(() => {
      raf = 0;
      const y = window.scrollY;
      if (y !== lastY && root.value) {
        lastY = y;
        root.value.style.setProperty('--parallax', String(Math.min(y, 600) * 0.06));
      }
    });
  };
  window.addEventListener('scroll', onScroll, { passive: true });
});

onUnmounted(() => {
  if (onScroll) window.removeEventListener('scroll', onScroll);
  if (raf) cancelAnimationFrame(raf);
});
</script>

<template>
  <div ref="root" :class="styles.atmosphere" aria-hidden="true">
    <div :class="[styles.glow, styles.glowA]"></div>
    <div :class="[styles.glow, styles.glowB]"></div>
    <div :class="styles.signal"></div>
    <span
      v-for="mote in motes"
      :key="mote.id"
      :class="styles.mote"
      :style="{
        left: mote.left + '%',
        width: mote.size + 'px',
        height: mote.size + 'px',
        animationDelay: mote.delay + 's',
        animationDuration: mote.duration + 's',
        '--drift': mote.drift + 'px',
      }"
    ></span>
  </div>
</template>