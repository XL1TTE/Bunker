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
A card revealed at the start of the game that defines the Bunker's constraints, most importantly its capacity.

### Bunker Capacity
The maximum number of players allowed to enter the Bunker, as defined by the Bunker Card.

### Group Survival
The win condition where multiple players can win by being voted into the Bunker.

### Elimination
The process where players are voted out of the game (left outside the Bunker) round by round.

### Survivor
A player who successfully makes it into the Bunker at the end of the game.

### AI Bot
A non-human player powered by a Large Language Model (LLM), configured with specific personality presets and behavior patterns.

### Character Sheet
The collection of attributes assigned to a player at the start of the game. A standard sheet includes:
- **Profession**: What the player does for a living.
- **Health**: Physical and mental state.
- **Hobby**: Personal interests or skills.
- **Luggage**: Items carried.
- **Special Trait**: A unique advantage or disadvantage.

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
An asynchronous choreographed saga initiated by the Host. 
1. The **Lobby Service** requests a game start by passing the IDs of the selected Card Packs and Bot Personalities.
2. The **Content Service** "hydrates" this request by fetching the full data for all requested IDs. It performs **Strict Validation**; if any ID is missing, the saga is aborted and the Lobby is notified.
3. If valid, the **Content Service** forwards the full content payload to the **Game Service** to prepare the session.
4. If preparation is successful, the Lobby is marked as "In Game" and clients are redirected.

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

### Game Action
A structured event (e.g., "Reveal Profession", "Vote") that has a direct impact on the game state and is distinct from free-form chat.
