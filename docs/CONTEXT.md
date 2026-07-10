# Bunker Game Context

## Glossary

### Game
A single session of the Bunker game, starting from lobby readiness and ending when the survivors are determined.

### Player
A participant in the game, either a human user or an AI bot.

### Player Profile
The persistent record of a human user's presence in the game, including their nickname, stats, and achievements.

### Keycloak-Linked Identity
The principle that a Player Profile is strictly tied to a unique Keycloak User ID, created automatically upon their first authenticated interaction.

### Aggregate Stats
High-level player metrics including total games played, total wins (survived), and total losses (eliminated). These stats are public and visible to any authenticated player.

### In-Game Nickname
A user-customizable display name used within the Bunker app, which defaults to the Keycloak username but can be modified.

### Bunker
The safe haven that a subset of players must enter to survive.

### Bunker Card
A card revealed at the start of the game that sets the stage for survival. It carries three fields:
- **Catastrophe** — what happened to the outside world.
- **Survival Duration** — the time horizon players must justify surviving (e.g. "10 years").
- **Bunker Environment** — a free-prose description of what the bunker has and lacks
  (supplies, area, surface conditions). This is the **usefulness lens**: players argue
  the value of their Attribute Cards *relative to* this environment (a food-rich bunker
  makes food luggage worthless; an irradiated surface makes radiation resistance gold).

The Bunker Card is **generated per game to ensure uniqueness**. Bunker Capacity is not
part of the card itself — it is derived from the participant count and displayed
alongside it.

### Bunker Capacity
The maximum number of players allowed to enter the Bunker. It is **derived from the
participant count** (not defined by the Bunker Card) and shown alongside the card.
At game start a survival ratio is picked at random from **30%, 40%, or 50%** of the
participant count (rounded to the nearest whole player, minimum 1); capacity is that
ratio applied to the initial participant count, fixed for the whole game.

### Group Survival
The win condition where multiple players can win by being voted into the Bunker.

### Elimination
The process where players are voted out of the game (left outside the Bunker) round by round.

### Survivor
A player who successfully makes it into the Bunker at the end of the game.

### AI Bot
A non-human player powered by a Large Language Model (LLM), configured with specific personality presets and behavior patterns.

### Character Sheet
The collection of attributes assigned to a player at the start of the game. Each
attribute is a distinct card type served by the Content Service. A standard sheet
includes seven attributes:
- **Profession**: What the player does for a living.
- **Sex**: Biological sex.
- **Age**: How old the player is.
- **Health**: Physical and mental state.
- **Hobbies**: Personal interests or skills.
- **Luggage**: Items carried.
- **Fact**: A unique condition or circumstance (replaces the earlier "Special Trait"
  concept, which is deferred).

`Health` and `Luggage` are new card types being added to the Content Service; the
`Special Trait` card type is intentionally out of scope for this iteration.

### Attribute Card
A specific piece of information on a Character Sheet that players reveal during the game to justify their entry into the Bunker.

### Public Event
An event intended for all players in a specific Game or Lobby (e.g., "Player A revealed their Profession").

### Private Event
An event intended for a specific player only (e.g., "Your secret cards have been assigned").

### Lobby
A temporary gathering place for players before a Game starts. Lobbies handle matchmaking, team composition, and readiness checks.

### Lobby Participant
An entity representing a Player's presence within a specific Lobby. It tracks lobby-specific state like role and readiness. In the domain model, this is often referred to simply as a Player within the Lobby context.

### Bot
A non-human player entity in the Lobby, configured with specific personality presets and behavior patterns. Bots do not require readiness checks but occupy capacity slots.

### Lobby Host
The participant who has the authority to change lobby settings (e.g., capacity, bots, packs) and initiate the Game once all participants are ready.

### Lobby Readiness
A binary state for each participant indicating they are prepared to start the game. A Game can only be initiated when all participants (excluding bots) are Ready.

### Card Pack
A curated set of Character Sheet attributes (Professions, Health, Hobbies, etc.) and Bunker Cards. Lobbies can include multiple packs (e.g., "Default", "18+", "Sci-Fi") to vary the game content.

### Lobby Capacity
The maximum total number of entities (Players + Bots) allowed in the Lobby.

### Lobby Slot
A position in a Lobby that can be occupied by either a human Player (joined
via invite code) or a Bot (added by the Host from a personality preset). Slots
are uniform — there is no separate "player section" or "bot section". The
distinction between a Player-occupied and Bot-occupied slot is the occupant's
type, plus the fact that Bots auto-count as Ready (no readiness toggle).

### Lobby Handoff
An asynchronous saga initiated by the Host, orchestrated by the **Game Service** (which owns the saga and session lifecycle). The Lobby stays thin — it holds opaque content references and does **not** subscribe to content update events; validation happens on-demand at game start against the source of truth (Content Service). The saga is **sequential** (AI first, canned only as compensation), so an AI-generation call is never wasted beside a failing hydration.
1. The Host calls `POST /lobbies/{id}/start` (returns **204** immediately). The **Lobby Service** validates host + readiness + participant count, transitions the lobby to `Starting`, and publishes a `GameStartRequested` event carrying the selected Card Pack IDs, Bot Personality Preset IDs, and participants.
2. The **Game Service** saga starts and requests **lightweight pack transfers** from the **Content Service** — only each pack's `Name` and `GenerationPrompt` (the LLM-facing description), not the canned cards. The Content Service performs **Strict Validation**; if any pack ID is missing the saga fails fast.
3. With the pack prompts in hand, the Game Service sends **one consolidated request** to the **AI Service** (Python, over RabbitMQ) to generate, themed to the selected packs and coherent with each other: **(a)** a Bunker Card, and **(b)** one Character Sheet per participant.
4. **On AI success**, the Game Service builds the `GameSession` (assigns the sheets to participants, attaches the bunker card) and publishes `GameStartSucceeded`.
5. **On AI failure after retries**, the saga runs **canned compensation**: it issues a `RequestGameContentHydration` to the Content Service, which returns all canned cards from the selected packs **plus a canned Bunker Card**; the Game Service assigns canned cards into per-participant sheets, then publishes `GameStartSucceeded`. If this compensation hydration also fails (a pack ID is missing), the saga publishes `GameStartFailed`.
6. The **Lobby Service** handles the outcome: `GameStartSucceeded` marks the lobby `InGame` and broadcasts `HandoffStarted` (carrying the `gameSessionId`) to the lobby's SignalR group; `GameStartFailed` reverts the lobby to `WaitingForPlayers` and broadcasts `GameStartFailed(reason)`. Clients are redirected to the **game screen only on success** — where the Bunker Card and each player's own Character Sheet are revealed for the first time. On failure they stay in the lobby and see the error.

### Content Management
The process of creating and maintaining the game's library of Cards, Card Packs, and Bot Personalities. In the current architecture, this is an **Admin-Only** operation. Changes to content are broadcasted via granular, full-state events (e.g., `FactCardUpdated`) to allow downstream services to update their caches.

### Lobby Destruction
The process where a Lobby and its associated data are deleted.
- **Pre-game**: Triggered immediately when the Host intentionally leaves. All participants are disbanded.
- **In-game**: Triggered ONLY when the last remaining player leaves the session. The session persists as long as at least one player is connected.

### Host Migration
The mechanism in an In-game Lobby where, if the current Host leaves intentionally, the "Host" status is transferred to the oldest remaining participant.

### Host Grace Period
A configurable duration (e.g., 2 minutes) during which a Lobby persists after a Host disconnects unintentionally. If the Host re-connects within this window, they resume their role; otherwise, Host Migration (In-game) or Lobby Destruction (Pre-game) occurs.

### Bot Auto-Eviction
the domain logic where Bots are automatically removed from a Lobby when the Host reduces the Capacity below the current total occupant count, provided that the number of human Players does not exceed the new limit.

### Topic-Based Routing
The mechanism where domain services publish events to specific Message Broker topics (e.g., `room.{id}` or `user.{id}`) which the Real-time Service uses to target recipients.

### Invite Code
A unique alphanumeric string generated for every Lobby. It is the primary mechanism for joining a Private Lobby and can also be used for direct joining of Public Lobbies.

### Lobby Browser
A feature allowing players to find and join Public Lobbies that are looking for more participants.

### State Checkpoint
A durable snapshot of the game state saved to PostgreSQL at the end of each round to allow for recovery in case of system failure.

### Discussion Chat
A real-time, free-form text communication channel for players to persuade others during the Discussion Phase.

### Game Round
A single pass of the gameplay loop. The game is **automatic and turn-based**, advancing on per-player and group timers. In a turn-based phase, each non-eliminated participant takes a turn; a turn ends when the player acts or their 1-minute timer expires, at which point an **auto-action** fires (e.g. a random attribute auto-opens, or a vote is abstained) and the next participant's turn begins. A phase ends when every non-eliminated participant has taken their turn.

**Round 1 (special)** opens with two extra phases before the regular cycle:
1. **Bunker Introduction** — the Bunker Card is shown to everyone.
2. **Intro Discussion** — turn-based, 1 min/player to introduce themselves.

**Every round** (the rest of round 1, and all rounds 2+) then runs:
3. **Reveal** — turn-based, 1 min/player to choose an attribute to reveal. Reveal is allowed **only in this phase**. On timeout a random unrevealed attribute auto-opens; a turn is auto-skipped if the player has no unrevealed attributes left.
4. **Discussion** — a 2-minute free-for-all chat, then turn-based 1 min/player for each player to make a closing case.
5. **Voting** — turn-based, 1 min/player to Vote for any other player. Votes are a **secret ballot** — only a "player has voted" progress signal is public during the phase; the full tally is revealed at Elimination. On timeout the vote is abstained (skipped).
6. **Elimination** — the max-vote player is eliminated. A tie triggers a second voting round; a second tie resolves via the **Roulette**.

After elimination, if the remaining players ≤ Bunker Capacity the game enters **Finished** (Survivors win, the rest lose); otherwise the next round begins. **Bots** take their turns with mock-random behavior (random attribute reveal, random vote) so the automatic flow proceeds with bots present.

### Roulette
The tie-break mechanic shown when a second voting round still produces no single max-vote player. A modal appears on every player's screen showing the tied players' avatars (nicknames) against each other; a highlight bounces between them fast, slowing over time, until it **stops on the player to be eliminated**. The winner is chosen at random from the tied (max-vote) players; the animation is cosmetic.

### Game Action
A structured event (e.g., "Reveal Profession", "Vote") that has a direct impact on the game state and is distinct from free-form chat.
