# bunker-frontend

Vue 3 + Vite + TypeScript lobby frontend for the Bunker game.

## Status

**MVP scope:** lobby bounded context only — authentication, lobby creation,
lobby browser, lobby room with real-time chat, host settings (capacity,
public/private, card packs, bots, kicking, starting the game).

Game-phase work (character sheets, reveals, voting) is deferred.

## Stack

- Vue 3 + Vite + TypeScript
- Pinia for state
- Vue Router for routing
- CSS Modules (`*.module.css`) for styling — applied via `:class="styles.x"`,
  scoped via Vite's built-in hashing. No `<style>` blocks in SFCs.
- Keycloak (`keycloak-js`) for authentication
- SignalR (`@microsoft/signalr`) for real-time lobby updates
- No component library — small set of hand-rolled primitives

## Quick start

```powershell
npm install
npm run dev          # http://localhost:5173
```

Before running, edit `.env` and confirm `VITE_API_BASE_URL` matches the
gateway (placeholder for now). For mock-only exploration leave
`VITE_USE_MOCKS=true` and you don't need anything else.

## Project layout

```
src/
  api/              # HTTP + realtime clients (interfaces + HTTP impls)
  auth/             # Keycloak wrapper
  components/       # Reusable UI
    common/
    lobby/
  mocks/            # In-memory mock implementations + seed data
  router/           # Vue Router config
  stores/           # Pinia stores (auth, lobby, catalog)
  styles/           # CSS Modules tokens + global styles
  types/            # Shared TS types (mirror backend DTOs)
  views/            # Route-level pages
  main.ts           # bootstrap
```

## The DI seam

The single most important architectural choice: every backend-touching module
goes through an interface, registered in `src/api/register.ts`. There are
two implementations of each interface:

- **HTTP/SignalR** — `AccountApi.http.ts`, `ContentApi.http.ts`,
  `LobbyApi.http.ts`, `LobbyRealtime.signalr.ts`
- **In-memory mocks** — `mocks/AccountApi.mock.ts`, `mocks/ContentApi.mock.ts`,
  `mocks/LobbyApi.mock.ts`, `mocks/LobbyRealtime.mock.ts`

Which one is registered is decided by `VITE_USE_MOCKS` in `.env` — **but not
by branching in code**. `register.ts` reads the env flag once and constructs
the right concrete type. Components and stores depend only on the interface,
so they don't change when the implementation swaps.

This means you can ship UI work with all mocks on, then turn individual
endpoints live one by one as the backend lands them, by editing only
`.env` and (later) `register.ts` to do fine-grained per-interface swaps.

## Where to look first

- `src/types/` — what the backend is supposed to return (vs. what the
  frontend needs). Compare to `docs/frontend/api-requirements.md`.
- `src/api/ILobbyApi.ts` and `src/stores/lobby.store.ts` — the lobby
  state machine in code form.
- `src/views/LobbyRoomView.vue` — the most complex view; everything
  else is small in comparison.

## Backend dependencies

See `docs/frontend/api-requirements.md` in the repo root for the full
list of endpoints the frontend expects. Until those endpoints exist,
everything in the UI is wired to mocks.