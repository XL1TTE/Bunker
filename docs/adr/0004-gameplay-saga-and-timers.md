# Gameplay as a long-lived, EF-mapped Wolverine saga with scheduled timers

The running Bunker game is long-lived, stateful, and **automatic/turn-based**:
per-player 1-minute turns with auto-actions on timeout, a 2-minute free-for-all
discussion, phase auto-advance, and a server-driven tie-break roulette. We model
it as a **separate long-lived `GameSaga`** (the short-lived `GameStartSaga` hands
off via a `StartGame` message), **EF-mapped in `GameDbContext`** — folding the old
`GameSessionEntity` into one entity with scalar columns plus JSONB columns for
nested state (participants + sheets + revealed flags, votes, tied players, bunker
card). Reads (`GET /game/{id}`) load the EF entity directly and map a
player-perspective transfer, matching the lobby pattern.

Timers are **Wolverine scheduled (durable, delayed) `TurnTimeout` messages**:
each turn schedules a timeout; when it fires, the handler applies the auto-action
(random reveal / abstain / advance) **idempotently** — only if `Phase` /
`CurrentTurnIndex` still match, so a player who already acted or a phase that
already advanced makes the timeout a no-op. Player Game Actions (Reveal, Vote) are
**HTTP endpoints** dispatching Wolverine commands to the saga; a `/hubs/game`
SignalR hub broadcasts Public/Private events (bunker card public; each player's
sheet private; reveals/phase-turn changes public). Votes are a **secret ballot**
(only a "player voted" signal is public; the full tally is revealed at
elimination); a tie triggers a second voting round, a second tie resolves via a
**roulette** whose eliminated player is chosen server-side and broadcast as
`RouletteResult` ~5s after `RouletteStarted` (the on-screen highlight animation is
cosmetic and client-side). Bots take their turns with mock-random behavior
(random reveal, random vote) so the automatic flow proceeds with bots present.

## Considered options

- **Domain aggregate + external/in-memory scheduler** — rejected: in-memory timers
  aren't durable (a restart stalls the game), and an external scheduler
  (Hangfire/Quartz) adds a dependency and a cross-replica ownership problem.
  Wolverine scheduled messages are already durable in the Postgresql message store
  and need no new infra.
- **Lightweight JSON saga storage (Wolverine default)** — rejected in favor of
  EF-mapped saga: EF Core saga persistence is auto-discovered from the registered
  `DbContext`, keeps state queryable (e.g. lobby→game by `LobbyId`), and allows
  direct repository reads like the lobby instead of routing reads through the saga.
- **SignalR hub methods for Reveal/Vote** — rejected in favor of HTTP endpoints:
  matches the lobby pattern, gives FluentValidation + proper status codes, and keeps
  the hub purely for broadcasts.
- **Public/live votes** — rejected in favor of secret ballot for strategic depth.

## Consequences

- The saga state is a single EF entity; nested collections are JSONB, rewritten in
  full on each save (fine for a minutes-long party game with ~10s of mutations).
- Every turn schedules a durable timeout message, so the game survives Game Service
  restarts/crashes mid-round and resumes on recovery.
- Discussion chat is **not** saga state — persisted as a separate `GameChatMessage`
  entity and broadcast via the hub.
- The frontend uses real `IGameApi` (HTTP) + `IGameRealtime` (SignalR) impls with no
  mock — game-screen development runs against the live Aspire stack.