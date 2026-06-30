import Keycloak from 'keycloak-js';

let keycloakInstance: Keycloak | null = null;

function initKeycloak(): Keycloak {
  if (keycloakInstance) return keycloakInstance;
  keycloakInstance = new Keycloak({
    url: import.meta.env.VITE_KEYCLOAK_URL,
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