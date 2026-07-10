<script setup lang="ts">
import { auth } from '@/auth/keycloak';
import styles from '@/views/access-denied.module.css';

function logout(): void {
  void auth.logout();
}
</script>

<template>
  <section :class="styles.wrap">
    <div :class="styles.card">
      <h1 :class="styles.title">You need the content admin role</h1>
      <p :class="styles.body">
        You're signed in, but your account doesn't have the <code>content-service.admin</code>
        role in the Bunker realm — that role is required to manage content here.
      </p>
      <p :class="styles.body">Ask someone with realm admin access to assign it to your account, then come back.</p>
      <div :class="styles.actions">
        <button class="btn btnGhost" @click="logout">Log out</button>
      </div>
    </div>
  </section>
</template>