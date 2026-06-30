# Frontend → Backend API Requirements

This document describes every endpoint the lobby MVP frontend needs from the
backend. All endpoints are reached through the gateway (single base URL);
internal service routing is the backend's concern.

Scope: **lobby bounded context only**. Game-phase endpoints (reveals, voting,
character sheets, etc.) are intentionally out of scope and will be specified
in a later document.

Legend:
- **[A]** — authenticated, any role
- **[AH]** — authenticated, must be the Lobby Host
- **Status** — HTTP status codes the frontend expects to handle

---

## AccountService

Already partially implemented (`/account/me`, `/account/{id}` in
`Bunker.AccountService/src/Endpoints/Account/`). Two gaps.

### `GET /account/me`
- **Auth**: [A]
- **Returns**: `PlayerProfileResponse` (current record is missing `Id`)
- **Required field changes**: response must include the player's stable
  `Id` (Guid) — the frontend uses it to recognise "this is me" in lobby
  member lists, to drive ownership-based UI (kick, start), and as the
  join-token identity. Current `PlayerProfileResponse` is a struct with
  only `Nickname, TotalGames, Wins, Losses`.
- **Status**: 200, 401

### `GET /account/{id}`
- **Auth**: [A]
- **Returns**: same `PlayerProfileResponse` shape (with `Id`)
- **Use**: shown inline next to participants in lobby rooms
- **Status**: 200, 401, 404

### `PATCH /account/me` *(new)*
- **Auth**: [A]
- **Body**: `{ nickname: string }` (validated server-side; existing
  `Nickname` domain type enforces the 4+ char rule)
- **Returns**: updated `PlayerProfileResponse`
- **Use**: nickname editor in account menu
- **Status**: 200, 400, 401

---

## LobbyService

No endpoints exist yet (`LobbyEndpoints.cs` is an empty stub). All of the
following need to be created. Recommended route group: `/lobbies`.

### `POST /lobbies` — Create lobby
- **Auth**: [A]
- **Body**: `{ capacity: int, isPublic: bool, selectedPackIds: Guid[] }`
- **Validation**: capacity ≥ 4 (matches `LobbyFactory` invariant),
  selectedPackIds may be empty
- **Returns**: full lobby state (same shape as `GET /lobbies/{id}`),
  including the freshly-minted `inviteCode`
- **Side effect**: creator is auto-joined as Host with `NotReady` status
- **Status**: 201, 400, 401

### `POST /lobbies/{code}/join` — Join by invite code
- **Auth**: [A]
- **Returns**: full lobby state
- **Errors**: 404 (code unknown), 409 (lobby full / already a participant /
  lobby already started / destroyed), 410 (lobby destroyed mid-flight)
- **Side effect**: caller is added as `PlayerParticipant` with role `Member`,
  status `NotReady`; `ParticipantJoined` event broadcast via SignalR hub
- **Status**: 200, 401, 404, 409

### `POST /lobbies/{id}/leave` — Leave intentionally
- **Auth**: [A]
- **Behaviour**: caller is removed from participants;
  - If caller was Host → lobby is destroyed immediately (pre-game rule),
    `LobbyDestroyed` event broadcast to all remaining connections (which
    the frontend will receive before they see 200, see "event ordering"
    below)
  - If caller was a Member → `ParticipantLeft` event broadcast
- **Returns**: 204 on success. If the lobby was destroyed, 200 with a
  body flag `destroyed: true` is also acceptable — but the SignalR event
  is the source of truth either way.
- **Status**: 204, 401, 404

### `GET /lobbies/{id}` — Full lobby state
- **Auth**: [A] — caller must be a participant OR the lobby is Public
- **Returns**: full lobby snapshot — see "Lobby snapshot shape" below
- **Use**: initial room load, reconnect after WS drop
- **Status**: 200, 401, 403, 404

### `GET /lobbies` — Public lobby browser
- **Auth**: [A]
- **Query**: `?limit=50&offset=0` (default 50, max 100)
- **Returns**: `{ items: LobbySummary[], total: int }` (LobbySummary is the
  lightweight shape — see below)
- **Use**: lobby browser
- **Status**: 200, 401

### `PATCH /lobbies/{id}/settings` — Host updates settings
- **Auth**: [AH]
- **Body**: any of `{ capacity?: int, isPublic?: bool, selectedPackIds?: Guid[] }`
  (partial update, at least one field required)
- **Validation**: capacity ≥ 4 AND ≥ current participant count; capacity
  changes that would exceed current occupant count must trigger
  **Bot Auto-Eviction** (drop bots until count ≤ new capacity); selected
  pack IDs must all exist in ContentService (Strict Validation)
- **Returns**: updated lobby state
- **Side effect**: `SettingsChanged` event broadcast
- **Status**: 200, 400, 401, 403, 404, 409 (invalid pack id)

### `POST /lobbies/{id}/bots` — Host adds bot
- **Auth**: [AH]
- **Body**: `{ personalityPresetId: Guid, nickname: string }`
- **Validation**: a free slot exists; preset id exists in ContentService
- **Returns**: updated lobby state
- **Side effect**: `BotAdded` event broadcast; bot is auto-Ready
- **Status**: 200, 400, 401, 403, 404, 409 (no free slot)

### `DELETE /lobbies/{id}/bots/{participantId}` — Host removes bot
- **Auth**: [AH]
- **Side effect**: `BotRemoved` event broadcast
- **Status**: 204, 401, 403, 404

### `DELETE /lobbies/{id}/participants/{participantId}` — Host kicks player
- **Auth**: [AH] — host cannot kick themselves; use `/leave` instead
- **Side effect**: `ParticipantKicked` event broadcast
- **Status**: 204, 401, 403, 404

### `POST /lobbies/{id}/ready` — Toggle own readiness
- **Auth**: [A] — caller must be a player participant (bots don't call this)
- **Behaviour**: flips the caller's status between Ready and NotReady
- **Returns**: updated lobby state
- **Side effect**: `ReadinessChanged` event broadcast
- **Status**: 200, 401, 403 (bot tried to ready), 404

### `POST /lobbies/{id}/start` — Initiate Lobby Handoff
- **Auth**: [AH]
- **Preconditions**: all human participants are Ready, lobby has at least
  one player participant
- **Side effect**: kicks off the Lobby Handoff saga. Returns 202 Accepted
  immediately; completion is signalled by `HandoffStarted` SignalR event.
  After `HandoffStarted`, the frontend redirects to the game screen (out
  of MVP scope).
- **Status**: 202, 400 (not all ready), 401, 403, 404, 409 (already started)

### SignalR hub `/hubs/lobby`
- **Auth**: [A] via standard Keycloak JWT query-string token (SignalR
  doesn't easily negotiate headers)
- **Client→server methods**:
  - `JoinLobby(lobbyId: Guid)` — server validates the caller is a
    participant (or creator if lobby is Public and caller isn't yet a
    participant, depending on chosen policy — recommend "must already be
    a participant; use HTTP join first"); adds the connection to the
    lobby's group
  - `LeaveLobby(lobbyId: Guid)` — opposite
- **Server→client events** (broadcast to lobby group, payload is JSON):
  - `ParticipantJoined { participant: PlayerParticipant }`
  - `ParticipantLeft { participantId: Guid }`
  - `ParticipantKicked { participantId: Guid, byHostId: Guid }`
  - `BotAdded { bot: BotParticipant }`
  - `BotRemoved { participantId: Guid }`
  - `SettingsChanged { lobby: LobbySnapshot }` — full lobby sent (matches
    the granular full-state pattern already used for content updates)
  - `ReadinessChanged { participantId: Guid, status: 'Ready'|'NotReady' }`
  - `ChatMessageReceived { id: Guid, participantId: Guid, nickname: string,
    text: string, sentAt: ISO8601 }`
  - `LobbyDestroyed { reason: 'HostLeft'|'CapacityChanged'|'Kicked' }`
  - `HandoffStarted { gameSessionId: Guid }` — frontend redirects on receipt
- **Reconnect behaviour**: on SignalR auto-reconnect, the client must
  re-call `JoinLobby(lobbyId)`. The frontend will do this automatically.

### `POST /lobbies/{id}/messages` — Send chat message
- **Auth**: [A], participant only
- **Body**: `{ text: string }` (1–500 chars, server-validated)
- **Side effect**: persists the message and broadcasts `ChatMessageReceived`
- **Returns**: 202 Accepted (the broadcast is the source of truth)
- **Status**: 202, 400, 401, 403, 404

---

## ContentService

Endpoints exist but are all admin-only (`Roles = "content-service.admin"`).
The lobby MVP needs **authenticated, non-admin read** endpoints to populate
host pickers.

### `GET /content/packs` — List card packs (new public read)
- **Auth**: [A]
- **Returns**: list of `CardPackPreview { id: Guid, title: string, description: string }`
  (lighter than the admin detail shape — frontend only needs preview data
  for the host's pack picker)
- **Status**: 200, 401
- **Note**: existing `GET /card-packs` should be left as-is for admins; this
  is a new endpoint with the lightweight projection

### `GET /content/personalities` — List bot personality presets (new public read)
- **Auth**: [A]
- **Returns**: list of `PersonalityPresetPreview { id: Guid, title: string, description: string }`
- **Status**: 200, 401
- **Note**: same pattern — existing admin `GET /personality-presets` stays;
  add a new authenticated-non-admin read

---

## Lobby snapshot shape

This is the response shape used by `GET /lobbies/{id}`, returned by most
mutating endpoints, and emitted inside `SettingsChanged` SignalR events.

```
LobbySnapshot {
  id: Guid
  inviteCode: string
  capacity: int
  isPublic: bool
  hostParticipantId: Guid
  participants: Participant[]   // both players and bots in slot order
  selectedPackIds: Guid[]
}

Participant {                    // tagged union — use a discriminator
  id: Guid
  nickname: string
  role: 'Host' | 'Member'
  status: 'Ready' | 'NotReady'
  type: 'Player' | 'Bot'
  // Player-only:
  accountId?: Guid               // absent for bots
  // Bot-only:
  personalityPresetId?: Guid     // absent for players
}
```

`LobbySummary` (for `GET /lobbies` browser) is a strict subset:
```
LobbySummary {
  id: Guid
  inviteCode: string
  capacity: int
  currentPlayers: int            // = participants.length
  hostNickname: string
  selectedPackIds: Guid[]
}
```

---

## Event ordering guarantee

When the Host intentionally leaves a pre-game lobby via `POST /lobbies/{id}/leave`,
the rest of the lobby must receive `LobbyDestroyed` *before* the leaving
client receives its HTTP response. This is so the leaving client can also
tear down its SignalR connection cleanly. If this can't be guaranteed,
the HTTP response should return `{ destroyed: true }` and the client treats
that as the destroy signal.

---

## Things the frontend does NOT need from the backend (yet)

These exist on the backend but the lobby MVP doesn't consume them:

- Card CRUD endpoints (only `GET` of packs/personalities is needed)
- Account creation (handled by `PlayerProfileSyncMiddleware` on first auth)
- All event-sourced content `*Updated` fanout topics
- Admin auth (`content-service.admin` role)
- Anything GameService-related

---

## Recommended order of implementation

1. `GET /account/me` enhancement (add `Id` to response)
2. Lobby endpoints 1-by-1 in this order: `POST /lobbies`, `GET /lobbies/{id}`,
   `GET /lobbies`, `POST /lobbies/{code}/join`, `POST /lobbies/{id}/leave`,
   `PATCH /lobbies/{id}/settings`, `POST /lobbies/{id}/ready`, then bot/kick/start.
3. SignalR hub `/hubs/lobby` (full event set) — needed for chat.
4. ContentService public reads: `GET /content/packs`, `GET /content/personalities`.
5. `POST /lobbies/{id}/messages` (chat persistence).
6. `PATCH /account/me` (nickname editing — optional polish).

The frontend can be brought online after step 2 partially lands — the
DI seam lets you flip individual endpoints from mock to real without
touching component code.