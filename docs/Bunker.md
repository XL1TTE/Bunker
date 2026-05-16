## System Overview

This document outlines a distributed microservices architecture for a multiplayer Bunker game with AI bot support, featuring real-time gameplay, lobby management, and intelligent bot opponents.

---

## 1. Core Microservices Architecture

### 1.1 Microservices Breakdown

| Service | Responsibility | Technology Stack | Port |
|---------|----------------|------------------|------|
| **Auth Service** | User authentication, JWT token generation, user session management | Node.js + Express, Redis for token cache | 3001 |
| **Player Service** | Player profiles, statistics, achievements, user data persistence | Node.js + Express, PostgreSQL | 3002 |
| **Lobby Service** | Lobby creation, player joining, lobby state management, slot allocation | Node.js + Express, PostgreSQL, Redis | 3003 |
| **Game Service** | Core game logic, turn management, voting system, game state transitions | Node.js + Express, PostgreSQL, Redis | 3004 |
| **AI Bot Service** | Bot behavior, decision making, card strategy, proposal generation | Python (FastAPI), TensorFlow/PyTorch for ML | 3005 |
| **Real-time Service** | WebSocket connections, event broadcasting, live updates | Node.js + Socket.io | 3006 |
| **Matchmaking Service** | Queue management, lobby auto-fill, player matching algorithms | Node.js + Express, Redis | 3007 |
| **Game State Service** | Distributed state management, game snapshot persistence, state validation | Node.js, Redis (primary), PostgreSQL (backup) | 3008 |

---

## 2. Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                          CLIENT LAYER                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐             │
│  │   Web App    │  │  Mobile App  │  │  Desktop CLI │             │
│  │  (React)     │  │  (React Nat) │  │  (Node CLI)  │             │
│  └──────────────┘  └──────────────┘  └──────────────┘             │
└────────┬────────────────────────────────────┬────────────────────────┘
         │ HTTP REST + WebSocket              │
         │                                    │
┌────────▼────────────────────────────────────▼────────────────────┐
│                    API GATEWAY LAYER                             │
│  (Kong / Nginx - Rate limiting, routing, authentication)        │
└────────┬────────────────────────────────────┬────────────────────┘
         │                                    │
    ┌────▼────┐                        ┌─────▼──────┐
    │ REST    │                        │ WebSocket  │
    │ Routes  │                        │ Gateway    │
    └────┬────┘                        └─────┬──────┘
         │                                   │
┌────────┴──────┬──────────┬───────────┬───┴────┬───────────┐
│               │          │           │        │           │
▼               ▼          ▼           ▼        ▼           ▼
┌─────────────┐ ┌──────────┐ ┌────────┐ ┌─────────┐ ┌──────────┐
│Auth Service │ │ Lobby    │ │ Player │ │ Game    │ │Real-time │
│ (3001)      │ │ Service  │ │Service │ │Service  │ │Service   │
│             │ │(3003)    │ │(3002)  │ │(3004)   │ │ (3006)   │
└─────────────┘ └──────────┘ └────────┘ └─────────┘ └──────────┘
       │             │            │          │           │
       │             │            │          │           │
┌──────▼─────────────▼────────────▼──────────▼───────────▼────┐
│           MESSAGE BROKER (RabbitMQ / Kafka)                 │
│  Events: game.started | player.joined | player.voted       │
│          game.ended | lobby.created | game.state_changed   │
└──────┬──────────────────────────────────────────────────────┘
       │
┌──────▼────────────────────────────────────────────────────────┐
│                  SUPPORTING SERVICES LAYER                    │
│  ┌────────────┐  ┌─────────────┐  ┌──────────────┐          │
│  │ Matchmaking│  │ AI Bot      │  │ Game State   │          │
│  │ Service    │  │ Service     │  │ Service      │          │
│  │ (3007)     │  │ (3005)      │  │ (3008)       │          │
│  └────────────┘  └─────────────┘  └──────────────┘          │
└───────────────────────────────────────────────────────────────┘
       │                   │                    │
       │                   │                    │
┌──────▼─────────┬─────────▼────────┬──────────▼──────────────┐
│  DATA LAYER   │                  │                         │
│ ┌────────────┐│ ┌──────────────┐ │ ┌────────────────────┐ │
│ │ PostgreSQL ││ │ Redis Cache  │ │ │ File Storage       │ │
│ │ (Players,  ││ │ (Sessions,   │ │ │ (Game replays)     │ │
│ │  Lobbies,  ││ │  Game state, │ │ │                    │ │
│ │  Games)    ││ │  Real-time   │ │ │                    │ │
│ │            ││ │  data)       │ │ │                    │ │
│ └────────────┘│ └──────────────┘ │ └────────────────────┘ │
└────────────────┴──────────────────┴──────────────────────────┘
```

---

## 3. Service-to-Service Communication Patterns

### 3.1 Synchronous Communication (HTTP REST)

```
CLIENT REQUEST FLOW:

┌────────┐                     ┌──────────────┐
│ Client │──────REST────────→  │API Gateway   │
└────────┘                     └───┬──────────┘
                                   │
                    ┌──────────────┼──────────────┐
                    ▼              ▼              ▼
                ┌────────┐    ┌────────┐    ┌────────┐
                │Service A│   │Service B│   │Service C│
                └────────┘    └────────┘    └────────┘
                    │              │              │
                    └──────────────┼──────────────┘
                                   │
                    ┌──────────────▼──────────────┐
                    │ Database / Cache Layer      │
                    └────────────────────────────┘
```

**Example REST Endpoints:**

- `POST /api/v1/auth/login` → Auth Service
- `POST /api/v1/lobbies` → Lobby Service
- `GET /api/v1/lobbies/{id}` → Lobby Service
- `POST /api/v1/games/{id}/vote` → Game Service
- `GET /api/v1/players/{id}` → Player Service

### 3.2 Asynchronous Communication (Message Broker)

```
EVENT-DRIVEN ARCHITECTURE:

1. GAME STARTED EVENT:
   ┌────────────┐
   │Game Service│
   └─────┬──────┘
         │ Publish: game.started
         ▼
   ┌───────────────────┐
   │ Message Broker    │
   │ (RabbitMQ/Kafka)  │
   └─────┬─────────────┘
         │
    ┌────┴────┬─────────┐
    ▼         ▼         ▼
  ┌────────┐┌──────┐┌──────────┐
  │Real-tm ││Lobby ││AI Bot    │
  │Service ││Serv. ││Service   │
  └────────┘└──────┘└──────────┘

2. PLAYER VOTED EVENT:
   ┌───────────┐
   │Game Serv. │ → Publish: player.voted
   └───────────┘
        │
        ▼
   ┌──────────────┐
   │Msg Broker    │
   └──────┬───────┘
          │
   ┌──────┴──────┐
   ▼             ▼
┌─────────┐  ┌──────────┐
│Game     │  │Real-time │
│State    │  │Service   │
│Service  │  │(Broadcast│
└─────────┘  │ Update)  │
             └──────────┘
```

**Event Types Published to Message Broker:**

| Event | Source | Subscribers | Purpose |
|-------|--------|-------------|---------|
| `game.started` | Game Service | Real-time, AI Bot | Initialize game, start AI strategies |
| `player.joined` | Lobby Service | Real-time, Game State | Update lobby UI, sync state |
| `player.voted` | Game Service | Real-time, Game State | Update vote count, broadcast result |
| `round.started` | Game Service | Real-time, AI Bot | Notify next turn, trigger AI decisions |
| `player.eliminated` | Game Service | Real-time, Matchmaking | Update player status, adjust ratings |
| `game.ended` | Game Service | Real-time, Player | Send final results, update stats |
| `proposal.made` | Game Service | Real-time, AI Bot | Show proposal to all, trigger AI voting |

### 3.3 Real-Time Communication (WebSocket)

```
CLIENT ↔ REAL-TIME SERVICE (Socket.io):

┌────────────────┐
│   Client 1     │
└────────┬───────┘
         │ WebSocket
         │
┌────────▼──────────────────────────┐
│  Real-time Service (Socket.io)   │
│  ┌──────────────────────────────┐│
│  │ Connection Manager           ││
│  │ • Track active connections   ││
│  │ • Room management            ││
│  │ • Event handlers             ││
│  └──────────────────────────────┘│
│  ┌──────────────────────────────┐│
│  │ Event Dispatcher             ││
│  │ • Listen to message broker   ││
│  │ • Transform events           ││
│  │ • Broadcast to rooms         ││
│  └──────────────────────────────┘│
└────────┬──────────────────────────┘
         │
    ┌────┴────┐
    ▼         ▼
┌────────┐┌────────────┐
│Client2 ││Message     │
│        ││Broker      │
└────────┘└────────────┘
```

**WebSocket Events Sent to Clients:**

- `game:player_joined` - New player entered lobby
- `game:turn_started` - Current player's turn begins
- `game:vote_cast` - Vote count updated in real-time
- `game:proposal` - Player's proposal displayed
- `game:discussion` - Discussion phase update
- `game:elimination` - Player eliminated
- `game:game_ended` - Game finished, show results
- `game:connection_error` - Connection lost, reconnecting

---

## 4. Detailed Service Specifications

### 4.1 Auth Service (Port 3001)

```javascript
// Endpoints
POST /auth/register
{
  "email": "user@example.com",
  "password": "securepass123",
  "username": "player_name"
}

Response:
{
  "user_id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "username": "player_name"
}

---

POST /auth/login
{
  "email": "user@example.com",
  "password": "securepass123"
}

Response:
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user_id": "550e8400-e29b-41d4-a716-446655440000",
  "expires_in": 3600
}

---

POST /auth/refresh
{
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}

Response:
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 3600
}

---

GET /auth/verify (with Authorization header)
Header: Authorization: Bearer {access_token}

Response:
{
  "valid": true,
  "user_id": "550e8400-e29b-41d4-a716-446655440000",
  "username": "player_name"
}
```

**Cache Strategy:** Redis with 30-minute TTL for token blacklist on logout

**Security:** JWT with HS256, password hashing with bcrypt (10 salt rounds)

---

### 4.2 Lobby Service (Port 3003)

#### Database Schema

```sql
CREATE TABLE lobbies (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  owner_id UUID NOT NULL,
  name VARCHAR(255) NOT NULL,
  description TEXT,
  status ENUM('waiting', 'starting', 'in_progress', 'ended') DEFAULT 'waiting',
  max_players INT DEFAULT 6,
  current_players INT DEFAULT 0,
  ai_bots_count INT DEFAULT 0,
  difficulty_level ENUM('easy', 'medium', 'hard') DEFAULT 'medium',
  created_at TIMESTAMP DEFAULT NOW(),
  started_at TIMESTAMP,
  ended_at TIMESTAMP,
  game_id UUID,
  FOREIGN KEY (owner_id) REFERENCES users(id),
  FOREIGN KEY (game_id) REFERENCES games(id),
  INDEX idx_status (status),
  INDEX idx_owner (owner_id)
);

CREATE TABLE lobby_players (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  lobby_id UUID NOT NULL,
  user_id UUID,
  is_ai BOOLEAN DEFAULT false,
  ai_bot_id VARCHAR(50),
  player_name VARCHAR(255) NOT NULL,
  position INT,
  is_ready BOOLEAN DEFAULT false,
  joined_at TIMESTAMP DEFAULT NOW(),
  FOREIGN KEY (lobby_id) REFERENCES lobbies(id) ON DELETE CASCADE,
  FOREIGN KEY (user_id) REFERENCES users(id),
  UNIQUE (lobby_id, position),
  INDEX idx_lobby (lobby_id)
);
```

#### REST Endpoints

```javascript
// Get lobby details
GET /lobbies/{lobby_id}

Response:
{
  "lobby_id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Tournament Game",
  "owner_id": "550e8400-e29b-41d4-a716-446655440001",
  "status": "waiting",
  "current_players": 3,
  "max_players": 6,
  "ai_bots_count": 0,
  "difficulty_level": "hard",
  "players": [
    {
      "position": 1,
      "player_name": "Player1",
      "is_ai": false,
      "is_ready": true,
      "user_id": "550e8400-e29b-41d4-a716-446655440001"
    },
    {
      "position": 2,
      "player_name": "Player2",
      "is_ai": false,
      "is_ready": false,
      "user_id": "550e8400-e29b-41d4-a716-446655440002"
    }
  ],
  "created_at": "2026-05-08T10:30:00Z"
}

---

// Join existing lobby
POST /lobbies/{lobby_id}/join
Headers: Authorization: Bearer {access_token}

Response:
{
  "success": true,
  "position": 3,
  "message": "Successfully joined lobby"
}

---

// Leave lobby
POST /lobbies/{lobby_id}/leave
Headers: Authorization: Bearer {access_token}

Response:
{
  "success": true,
  "message": "Left lobby"
}

---

// Add AI bots to lobby
POST /lobbies/{lobby_id}/add-bots
{
  "bot_count": 3,
  "difficulty": "medium"
}

Response:
{
  "success": true,
  "bots_added": 3,
  "total_players": 5,
  "bots": [
    {
      "position": 4,
      "player_name": "AI_Bot_1",
      "is_ai": true,
      "ai_bot_id": "bot_uuid_1"
    },
    {
      "position": 5,
      "player_name": "AI_Bot_2",
      "is_ai": true,
      "ai_bot_id": "bot_uuid_2"
    },
    {
      "position": 6,
      "player_name": "AI_Bot_3",
      "is_ai": true,
      "ai_bot_id": "bot_uuid_3"
    }
  ]
}

---

// Player ready signal
POST /lobbies/{lobby_id}/ready
Headers: Authorization: Bearer {access_token}

Response:
{
  "success": true,
  "your_ready_status": true,
  "ready_count": 5,
  "total_players": 6
}

---

// Start game (lobby owner only)
POST /lobbies/{lobby_id}/start
Headers: Authorization: Bearer {access_token}

Response:
{
  "success": true,
  "game_id": "550e8400-e29b-41d4-a716-446655440100",
  "message": "Game started"
}
```

#### Service Communication

**Calls to Other Services:**

- **To Matchmaking Service:** `GET /recommend-bots?difficulty=medium&count=3` - Get AI bots to populate lobby
- **To Game Service:** `POST /games/create` - Create game when lobby is full and all players ready
- **To Real-time Service:** Emit via message broker `lobby.player_joined`, `lobby.player_left`, `lobby.ready_status_changed`

---

### 4.3 Game Service (Port 3004)

#### Game State Machine

```
STATE TRANSITIONS:

                    ┌──────────────────┐
                    │  LOBBY_ASSIGNED  │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │ WAITING_FOR_DECK │
                    │  (Load card deck)│
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │CARDS_DISTRIBUTED │
                    │ (All get cards)  │
                    └────────┬─────────┘
                             │
         ┌───────────────────┴───────────────────┐
         │                                       │
         ▼                                       ▼
┌─────────────────┐                   ┌─────────────────┐
│ ROUND_STARTED   │                   │  GAME_OVER      │
│ (next 5 rounds) │                   │ (1 winner left) │
└────────┬────────┘                   └─────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│   PROPOSAL_PHASE                    │
│   (Current player proposes entry)   │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  DISCUSSION_PHASE                   │
│  (30 seconds for discussion)        │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  VOTING_PHASE                       │
│  (Consensus vote on entry)          │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  ELIMINATION                        │
│  (Player with most votes out)       │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  ROUND_ENDED                        │
└────────┬────────────────────────────┘
         │
         └─── (repeat if players > 1) ──→ ROUND_STARTED
```

#### Database Schema

```sql
CREATE TABLE games (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  lobby_id UUID UNIQUE NOT NULL,
  status VARCHAR(50) DEFAULT 'setup',
  current_round INT DEFAULT 0,
  max_rounds INT DEFAULT 5,
  current_phase VARCHAR(50) DEFAULT 'waiting_for_cards',
  current_turn_player_id UUID,
  created_at TIMESTAMP DEFAULT NOW(),
  started_at TIMESTAMP,
  ended_at TIMESTAMP,
  winner_id UUID,
  FOREIGN KEY (lobby_id) REFERENCES lobbies(id),
  INDEX idx_status (status),
  INDEX idx_lobby (lobby_id)
);

CREATE TABLE game_players (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  game_id UUID NOT NULL,
  user_id UUID,
  is_ai BOOLEAN DEFAULT false,
  ai_bot_id VARCHAR(50),
  position INT NOT NULL,
  player_name VARCHAR(255) NOT NULL,
  card_character VARCHAR(255),
  card_profession VARCHAR(255),
  card_description TEXT,
  status ENUM('alive', 'eliminated', 'winner') DEFAULT 'alive',
  eliminated_round INT,
  votes_received INT DEFAULT 0,
  is_ready BOOLEAN DEFAULT false,
  FOREIGN KEY (game_id) REFERENCES games(id) ON DELETE CASCADE,
  FOREIGN KEY (user_id) REFERENCES users(id),
  UNIQUE (game_id, position),
  INDEX idx_game (game_id),
  INDEX idx_status (status)
);

CREATE TABLE game_votes (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  game_id UUID NOT NULL,
  round INT NOT NULL,
  voter_id UUID NOT NULL,
  voter_is_ai BOOLEAN DEFAULT false,
  voted_for_id UUID NOT NULL,
  vote_reason TEXT,
  created_at TIMESTAMP DEFAULT NOW(),
  FOREIGN KEY (game_id) REFERENCES games(id),
  INDEX idx_game_round (game_id, round),
  INDEX idx_voter (voter_id)
);

CREATE TABLE game_proposals (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  game_id UUID NOT NULL,
  round INT NOT NULL,
  proposer_id UUID NOT NULL,
  proposal_text TEXT NOT NULL,
  created_at TIMESTAMP DEFAULT NOW(),
  FOREIGN KEY (game_id) REFERENCES games(id),
  INDEX idx_game_round (game_id, round)
);
```

#### REST Endpoints

```javascript
// Initialize game from lobby
POST /games
Headers: Authorization: Bearer {access_token}
{
  "lobby_id": "550e8400-e29b-41d4-a716-446655440000"
}

Response:
{
  "game_id": "550e8400-e29b-41d4-a716-446655440100",
  "lobby_id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "setup",
  "created_at": "2026-05-08T10:35:00Z"
}

---

// Get game state
GET /games/{game_id}
Headers: Authorization: Bearer {access_token}

Response:
{
  "game_id": "550e8400-e29b-41d4-a716-446655440100",
  "status": "running",
  "current_round": 2,
  "max_rounds": 5,
  "current_phase": "voting_phase",
  "current_turn_player_id": "550e8400-e29b-41d4-a716-446655440001",
  "players": [
    {
      "position": 1,
      "player_name": "Player1",
      "status": "alive",
      "card_character": "Teacher",
      "card_profession": "Education",
      "votes_received": 0
    },
    {
      "position": 2,
      "player_name": "Player2",
      "status": "alive",
      "card_character": "Doctor",
      "card_profession": "Medicine",
      "votes_received": 2
    },
    {
      "position": 3,
      "player_name": "AI_Bot_1",
      "is_ai": true,
      "status": "eliminated",
      "eliminated_round": 1
    }
  ]
}

---

// Distribute cards to all players
POST /games/{game_id}/distribute-cards
Headers: Authorization: Bearer {access_token}

Response:
{
  "success": true,
  "message": "Cards distributed to all players",
  "your_card": {
    "character": "Engineer",
    "profession": "Technology",
    "description": "Skilled in technical systems and problem solving"
  }
}

---

// Make proposal for entry to bunker
POST /games/{game_id}/make-proposal
Headers: Authorization: Bearer {access_token}
{
  "proposal_text": "I am an engineer with survival skills. I can help maintain the bunker systems and ensure everyone survives.",
  "round": 2
}

Response:
{
  "success": true,
  "proposal_id": "550e8400-e29b-41d4-a716-446655440200",
  "message": "Proposal registered, discussion phase begins"
}

---

// Cast vote on proposal
POST /games/{game_id}/cast-vote
Headers: Authorization: Bearer {access_token}
{
  "voted_for_player_id": "550e8400-e29b-41d4-a716-446655440002",
  "vote_reason": "Not qualified enough for bunker",
  "round": 2
}

Response:
{
  "success": true,
  "vote_recorded": true,
  "current_votes": {
    "550e8400-e29b-41d4-a716-446655440002": 3,
    "550e8400-e29b-41d4-a716-446655440003": 2
  }
}

---

// Get current voting results
GET /games/{game_id}/voting-results
Headers: Authorization: Bearer {access_token}

Response:
{
  "round": 2,
  "phase": "voting_phase",
  "votes_received": {
    "550e8400-e29b-41d4-a716-446655440001": 1,
    "550e8400-e29b-41d4-a716-446655440002": 3,
    "550e8400-e29b-41d4-a716-446655440003": 2
  },
  "total_votes_cast": 6,
  "votes_needed": 6,
  "voting_complete": true
}

---

// End current round (automatically called when votes cast)
POST /games/{game_id}/end-round
Headers: Authorization: Bearer {access_token}

Response:
{
  "success": true,
  "eliminated_player": {
    "position": 2,
    "player_name": "Player2",
    "votes": 3
  },
  "next_round": 3,
  "remaining_players": 5
}

---

// Get game results (after game ends)
GET /games/{game_id}/results
Headers: Authorization: Bearer {access_token}

Response:
{
  "game_id": "550e8400-e29b-41d4-a716-446655440100",
  "status": "ended",
  "winner": {
    "position": 1,
    "player_name": "Player1",
    "user_id": "550e8400-e29b-41d4-a716-446655440001",
    "card_character": "Teacher"
  },
  "total_rounds": 4,
  "elimination_order": [
    {
      "round": 1,
      "player_name": "AI_Bot_1",
      "votes": 4
    },
    {
      "round": 2,
      "player_name": "Player2",
      "votes": 3
    }
  ],
  "ended_at": "2026-05-08T10:55:00Z"
}
```

---

### 4.4 AI Bot Service (Port 3005)

#### Architecture Overview

```
AI BOT SERVICE FLOW:

Input Events from Message Broker:
├── game.started → Initialize bot strategies
├── proposal.made → Analyze proposal, decide vote
├── round.started → Choose next action
└── voting.phase → Cast vote

Processing Pipeline:
┌─────────────────┐
│ Event Received  │
└────────┬────────┘
         │
         ▼
┌──────────────────────────────┐
│ Bot Personality Loading      │
│ • Difficulty level           │
│ • Playing style              │
│ • Behavior weights           │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ Context Analysis             │
│ • Current game state         │
│ • Player elimination history │
│ • Card analysis              │
│ • Voting patterns            │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ Decision Making (ML Model)   │
│ • Evaluate strategies        │
│ • Calculate probabilities    │
│ • Select best action         │
└────────┬─────────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ Action Execution             │
│ • Generate proposal text     │
│ • Cast vote                  │
│ • Send to Game Service       │
└──────────────────────────────┘
```

#### REST Endpoints

```javascript
// Get bot decision for proposal
POST /bots/{bot_id}/decide-vote
{
  "game_id": "550e8400-e29b-41d4-a716-446655440100",
  "proposal": {
    "proposer_id": "550e8400-e29b-41d4-a716-446655440002",
    "text": "I am engineer with survival skills..."
  },
  "game_state": {
    "round": 2,
    "players_alive": 5,
    "player_cards": {
      "550e8400-e29b-41d4-a716-446655440001": "Teacher",
      "550e8400-e29b-41d4-a716-446655440002": "Doctor"
    }
  }
}
