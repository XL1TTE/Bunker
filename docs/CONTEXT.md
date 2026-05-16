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
A non-human player powered by a Large Language Model (LLM), capable of participating in discussions, making proposals, and voting based on social context.

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

### Topic-Based Routing
The mechanism where domain services publish events to specific Message Broker topics (e.g., `room.{id}` or `user.{id}`) which the Real-time Service uses to target recipients.

### Invite Code
A unique alphanumeric string used to join a specific Private Lobby.

### Lobby Browser
A feature allowing players to find and join Public Lobbies that are looking for more participants.

### State Checkpoint
A durable snapshot of the game state saved to PostgreSQL at the end of each round to allow for recovery in case of system failure.

### Discussion Chat
A real-time, free-form text communication channel for players to persuade others during the Discussion Phase.

### Game Action
A structured event (e.g., "Reveal Profession", "Vote") that has a direct impact on the game state and is distinct from free-form chat.
