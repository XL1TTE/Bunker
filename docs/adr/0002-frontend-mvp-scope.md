# MVP scope: Lobby frontend only

The first frontend iteration is scoped strictly to the Lobby bounded context.
Authentication, lobby creation, lobby joining, lobby browser, lobby room with
real-time chat, host settings (capacity, public/private, card packs, bots,
kicking, starting the game). Game-screen work — character sheets, reveal
phase, voting phase, elimination — is deferred to a later iteration.

## Decisions captured in this grilling session

- **Stack**: Vue 3 + Vite + TypeScript, Pinia for state, Vue Router for
  routing. Styling via CSS Modules (`*.module.css` imported as objects,
  applied via `:class="styles.foo"`, scoped via Vite's built-in hashing).
  SFC `<style>` blocks are intentionally avoided.
- **Realtime**: SignalR hub scoped to lobby participation (`Option B` —
  connection survives route navigation, joined explicitly via
  `JoinLobby(lobbyId)` after `connection.start()`). Hub endpoint
  `/hubs/lobby`. Multi-tab same lobby: lenient (each tab is its own
  logical participant, server treats them as one).
- **Auth**: Keycloak-hosted login UI via `keycloak-js`. Backend exchanges
  the code for a JWT; frontend holds access token in memory (library
  default), refresh via silent SSO. Logout = Keycloak SSO logout.
- **API access**: All frontend HTTP/WS traffic goes through a single
  gateway. Frontend only knows one base URL. Until the gateway exists,
  the base URL is a `.env` placeholder.
- **Backend mocking**: Interface-based DI. Four interfaces
  (`IAccountApi`, `ILobbyApi`, `IContentApi`, `ILobbyRealtime`), each
  registered with a concrete impl. Mocks live in `mocks/*.mock.ts`,
  swap by changing one `app.provide(...)` line in `main.ts`. No env
  flag for mocking — DI swap is the seam.
- **Mock seed data**: 3–4 hardcoded lobbies, 1 pack, 1–2 bot presets.
- **Slots are uniform**: No "player section" / "bot section". Each slot
  is occupied by either a human (joined via invite code) or a bot
  (added by host from a preset). See `docs/CONTEXT.md` — Lobby Slot.
- **Bots in MVP**: Host can add bots to any empty slot. Bots auto-count
  as Ready. On game start, the selected preset IDs are passed in the
  start request (Lobby Handoff saga consumes them).
- **Public/private**: Toggled at creation. Public = appears in browser,
  joinable by code. Private = hidden from browser, joinable only by code.
- **Browser**: Flat list of public lobbies (code, current/total, pack
  names, host nickname, Join). No search/filter/sort. Pagination: first
  50, "Load more".
- **Profiles**: Inline only in lobby member list (e.g., nickname +
  games/wins). No dedicated profile page in MVP.
- **Pre-game host leaves**: Lobby destroyed immediately, all
  participants disbanded. No host migration pre-game.
- **Routes**:
  `/`, `/auth/callback`, `/lobbies`, `/lobbies/new`, `/lobbies/:id`,
  `/:catchAll(.*)`. Auth guard on lobby routes.
- **Lobby destroyed while in room**: Modal on top of room view,
  "Lobby was destroyed" + "Back to browser" button. No dedicated route.