import { defineStore } from 'pinia';
import { ref } from 'vue';

export type Theme = 'dark' | 'light';

const STORAGE_KEY = 'bunker:theme';

function bgFor(theme: Theme): string {
  return theme === 'light' ? '#D1E8E2' : '#2C3531';
}

/** Read the theme the inline script already wrote to <html data-theme>. */
function readDom(): Theme {
  return document.documentElement.dataset.theme === 'light' ? 'light' : 'dark';
}

export const useThemeStore = defineStore('theme', () => {
  // The inline script in index.html sets data-theme before the app boots, so
  // the store initializes from the DOM to stay in lockstep with first paint.
  const theme = ref<Theme>(readDom());

  function apply(next: Theme): void {
    theme.value = next;
    const root = document.documentElement;
    root.setAttribute('data-theme', next);
    root.style.backgroundColor = bgFor(next);
    try {
      localStorage.setItem(STORAGE_KEY, next);
    } catch {
      /* localStorage unavailable (private mode) — keep the in-memory value. */
    }
  }

  function toggle(): void {
    apply(theme.value === 'dark' ? 'light' : 'dark');
  }

  // Follow OS preference only while the user has not made an explicit choice.
  const mql = window.matchMedia('(prefers-color-scheme: light)');
  mql.addEventListener('change', (e) => {
    let explicit = false;
    try {
      explicit = localStorage.getItem(STORAGE_KEY) !== null;
    } catch {
      explicit = false;
    }
    if (!explicit) apply(e.matches ? 'light' : 'dark');
  });

  return { theme, toggle, set: apply };
});