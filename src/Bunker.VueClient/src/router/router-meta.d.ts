// RouteMeta augmentation. This file is a MODULE (the side-effect import makes
// it one), so the `declare module 'vue-router'` below is treated as an
// augmentation that MERGES with vue-router's real types. Declaring the same
// block in a non-module .d.ts (env.d.ts) would instead *replace* vue-router's
// types and break every `useRouter`/`RouterLink` import.
import 'vue-router';

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean;
    layout?: 'landing' | 'app';
  }
}