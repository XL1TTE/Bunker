<script setup lang="ts">
import { computed } from 'vue';
import { RouterLink, RouterView, useRoute } from 'vue-router';
import { auth } from '@/auth/keycloak';
import { CARD_TYPES } from '@/types/admin.types';
import styles from '@/components/app-shell.module.css';

const route = useRoute();
const username = computed(() => auth.username());

// A card-type nav entry is active when the :type route param matches its key.
function isCardTypeActive(key: string): boolean {
  return route.name === 'cards' && route.params.type === key;
}

function logout(): void {
  void auth.logout();
}
</script>

<template>
  <div :class="styles.shell">
    <aside :class="styles.sidebar">
      <div :class="styles.brand">
        <span :class="styles.logoMark" aria-hidden="true">B</span>
        <div :class="styles.brandText">
          <span :class="styles.brandName">Bunker</span>
          <span :class="styles.brandSub">content admin</span>
        </div>
      </div>

      <nav :class="styles.nav">
        <RouterLink to="/" :class="[styles.navLink, route.name === 'dashboard' && styles.navLinkActive]">
          Dashboard
        </RouterLink>

        <div :class="styles.navGroup">
          <span :class="styles.navGroupLabel">Character cards</span>
          <RouterLink
            v-for="t in CARD_TYPES"
            :key="t.key"
            :to="{ name: 'cards', params: { type: t.key } }"
            :class="[styles.navLink, styles.navLinkSub, isCardTypeActive(t.key) && styles.navLinkActive]"
          >
            {{ t.label }}
          </RouterLink>
        </div>

        <div :class="styles.navGroup">
          <span :class="styles.navGroupLabel">Game content</span>
          <RouterLink to="/bunker-cards" :class="[styles.navLink, styles.navLinkSub, route.name === 'bunker-cards' && styles.navLinkActive]">
            Bunker cards
          </RouterLink>
          <RouterLink to="/packs" :class="[styles.navLink, styles.navLinkSub, route.name === 'packs' && styles.navLinkActive]">
            Card packs
          </RouterLink>
          <RouterLink to="/presets" :class="[styles.navLink, styles.navLinkSub, route.name === 'presets' && styles.navLinkActive]">
            Bot presets
          </RouterLink>
        </div>
      </nav>

      <div :class="styles.userFooter">
        <div :class="styles.userChip">
          <span :class="styles.avatar" aria-hidden="true">{{ username[0]?.toUpperCase() ?? '?' }}</span>
          <span :class="styles.userName">{{ username }}</span>
        </div>
        <button :class="styles.logoutBtn" @click="logout">Log out</button>
      </div>
    </aside>

    <main :class="styles.main">
      <RouterView />
    </main>
  </div>
</template>