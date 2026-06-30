import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import { auth } from '@/auth/keycloak';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    component: () => import('@/views/HomeView.vue'),
  },
  {
    path: '/auth/callback',
    name: 'auth-callback',
    component: () => import('@/views/AuthCallbackView.vue'),
  },
  {
    path: '/lobbies',
    name: 'lobbies',
    component: () => import('@/views/LobbyBrowserView.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/lobbies/new',
    name: 'lobby-new',
    component: () => import('@/views/LobbyCreateView.vue'),
    meta: { requiresAuth: true },
  },
  {
    path: '/lobbies/:id',
    name: 'lobby-room',
    component: () => import('@/views/LobbyRoomView.vue'),
    meta: { requiresAuth: true },
    props: true,
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
  return true;
});