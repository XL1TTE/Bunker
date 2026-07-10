import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import { auth } from '@/auth/keycloak';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
  {
    path: '/cards/:type',
    name: 'cards',
    component: () => import('@/views/CardListView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
    props: true,
  },
  {
    path: '/bunker-cards',
    name: 'bunker-cards',
    component: () => import('@/views/BunkerCardsView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
  {
    path: '/packs',
    name: 'packs',
    component: () => import('@/views/PacksView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
  {
    path: '/presets',
    name: 'presets',
    component: () => import('@/views/PresetsView.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
  },
  {
    path: '/access-denied',
    name: 'access-denied',
    component: () => import('@/views/AccessDeniedView.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/auth/callback',
    name: 'auth-callback',
    component: () => import('@/views/AuthCallbackView.vue'),
  },
  {
    path: '/:catchAll(.*)',
    name: 'not-found',
    component: () => import('@/views/NotFoundView.vue'),
  },
];

export const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach(async (to) => {
  if (to.meta.requiresAuth && !auth.isAuthenticated()) {
    await auth.login(window.location.origin + to.fullPath);
    return false;
  }
  // Defense in depth: the backend is the authority (every /content mutation requires
  // content-service.admin), but bouncing a non-admin to a clear "no access" view beats
  // a wall of 403 toasts.
  if (to.meta.requiresAdmin && !auth.hasRealmRole('content-service.admin')) {
    return { path: '/access-denied' };
  }
  return true;
});