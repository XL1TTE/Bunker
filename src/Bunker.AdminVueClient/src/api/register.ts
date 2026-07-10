import type { App } from 'vue';
import { ContentAdminApiHttp } from './ContentAdminApi.http';
import { ContentAdminApiKey, type IContentAdminApi } from './IContentAdminApi';
import type { AuthTokenProvider } from './http';

export interface ApiContainer {
  admin: IContentAdminApi;
}

// Real-only by design: the admin panel talks to the real ContentService through the
// gateway. There is no mock layer (per the project's no-mock constraint for new slices).
function buildApiContainer(tokens: AuthTokenProvider): ApiContainer {
  return {
    admin: new ContentAdminApiHttp(tokens),
  };
}

let container: ApiContainer | null = null;

export function registerApi(app: App, tokens: AuthTokenProvider): void {
  container = buildApiContainer(tokens);
  app.provide(ContentAdminApiKey, container.admin);
}

export function getApiContainer(): ApiContainer {
  if (!container) {
    throw new Error('API container not initialised — call registerApi() before getApiContainer().');
  }
  return container;
}