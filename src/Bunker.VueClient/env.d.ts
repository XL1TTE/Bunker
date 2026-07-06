/// <reference types="vite/client" />

// Injected by vite.config.ts from the Aspire-provided Keycloak URL. See keycloak.ts.
declare const __KEYCLOAK_URL__: string | undefined;

declare module '*.vue' {
  import type { DefineComponent } from 'vue';
  const component: DefineComponent<{}, {}, any>;
  export default component;
}

declare module '*.module.css' {
  const classes: Record<string, string>;
  export default classes;
}

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  readonly VITE_KEYCLOAK_URL: string;
  readonly VITE_KEYCLOAK_REALM: string;
  readonly VITE_KEYCLOAK_CLIENT_ID: string;
  readonly VITE_USE_MOCKS: string;
  readonly VITE_SKIP_AUTH: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}