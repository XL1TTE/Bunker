import type { App } from 'vue';
import { AccountApiKey, type IAccountApi } from './IAccountApi';
import { ContentApiKey, type IContentApi } from './IContentApi';
import { LobbyApiKey, type ILobbyApi } from './ILobbyApi';
import { LobbyRealtimeKey, type ILobbyRealtime } from './ILobbyRealtime';
import { GameApiKey, type IGameApi } from './IGameApi';
import { GameRealtimeKey, type IGameRealtime } from './IGameRealtime';
import { AccountApiHttp } from './AccountApi.http';
import { ContentApiHttp } from './ContentApi.http';
import { LobbyApiHttp } from './LobbyApi.http';
import { LobbyRealtimeSignalR } from './LobbyRealtime.signalr';
import { GameApiHttp } from './GameApi.http';
import { GameRealtimeSignalR } from './GameRealtime.signalr';
import { AccountApiMock } from '@/mocks/AccountApi.mock';
import { ContentApiMock } from '@/mocks/ContentApi.mock';
import { LobbyApiMock } from '@/mocks/LobbyApi.mock';
import { LobbyRealtimeMock } from '@/mocks/LobbyRealtime.mock';
import type { AuthTokenProvider } from './http';

const useMocks = import.meta.env.VITE_USE_MOCKS === 'true';

export interface ApiContainer {
  account: IAccountApi;
  content: IContentApi;
  lobby: ILobbyApi;
  realtime: ILobbyRealtime;
  // The game slice is real-only by design (no mock): per the project constraint
  // "We don't need mock, we will use real backend right away." These are always
  // the HTTP/SignalR impls, regardless of VITE_USE_MOCKS.
  game: IGameApi;
  gameRealtime: IGameRealtime;
}

let container: ApiContainer | null = null;

export function getApiContainer(): ApiContainer {
  if (!container) throw new Error('API container not initialised. Call registerApi() first.');
  return container;
}

export function buildApiContainer(tokens: AuthTokenProvider): ApiContainer {
  return {
    account: useMocks ? new AccountApiMock() : new AccountApiHttp(tokens),
    content: useMocks ? new ContentApiMock() : new ContentApiHttp(tokens),
    lobby: useMocks ? new LobbyApiMock() : new LobbyApiHttp(tokens),
    realtime: useMocks ? new LobbyRealtimeMock() : new LobbyRealtimeSignalR(tokens),
    game: new GameApiHttp(tokens),
    gameRealtime: new GameRealtimeSignalR(tokens),
  };
}

export function registerApi(app: App, tokens: AuthTokenProvider): ApiContainer {
  container = buildApiContainer(tokens);
  app.provide(AccountApiKey, container.account);
  app.provide(ContentApiKey, container.content);
  app.provide(LobbyApiKey, container.lobby);
  app.provide(LobbyRealtimeKey, container.realtime);
  app.provide(GameApiKey, container.game);
  app.provide(GameRealtimeKey, container.gameRealtime);
  return container;
}