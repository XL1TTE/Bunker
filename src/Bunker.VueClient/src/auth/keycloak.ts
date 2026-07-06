import Keycloak from 'keycloak-js';

// Injected at build time by vite.config.ts from the Aspire-provided Keycloak URL
// (AUTH_HTTPS/AUTH_HTTP). Falls back to import.meta.env.VITE_KEYCLOAK_URL for
// standalone `npm run dev` without Aspire.
declare const __KEYCLOAK_URL__: string | undefined;

let keycloakInstance: Keycloak | null = null;

function keycloakUrl(): string {
  return __KEYCLOAK_URL__ ?? import.meta.env.VITE_KEYCLOAK_URL ?? 'http://localhost:8080';
}

function initKeycloak(): Keycloak {
  if (keycloakInstance) return keycloakInstance;
  keycloakInstance = new Keycloak({
    url: keycloakUrl(),
    realm: import.meta.env.VITE_KEYCLOAK_REALM,
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
  });
  return keycloakInstance;
}

const SKIP_AUTH = import.meta.env.VITE_SKIP_AUTH === 'true';

export const auth = {
  init(): Promise<boolean> {
    if (SKIP_AUTH) {
      return Promise.resolve(true);
    }
    return initKeycloak().init({
      onLoad: 'check-sso',
      silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
    });
  },

  login(redirectUri: string = window.location.origin): Promise<void> {
    if (SKIP_AUTH) return Promise.resolve();
    return initKeycloak().login({ redirectUri });
  },

  logout(redirectUri: string = window.location.origin): Promise<void> {
    if (SKIP_AUTH) return Promise.resolve();
    return initKeycloak().logout({ redirectUri });
  },

  getAccessToken(): string | null {
    if (SKIP_AUTH) return 'mock-token';
    return initKeycloak().token ?? null;
  },

  getAccessTokenAsync(): Promise<string | null> {
    if (SKIP_AUTH) return Promise.resolve('mock-token');
    const kc = initKeycloak();
    return kc.updateToken(30).then(() => kc.token ?? null).catch(() => null);
  },

  isAuthenticated(): boolean {
    if (SKIP_AUTH) return true;
    return !!initKeycloak().authenticated;
  },
};