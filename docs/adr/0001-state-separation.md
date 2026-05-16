# ADR 0001: Separation of Game Logic and Game State Services

## Context
The system architecture includes two distinct services: `Game Service` (3004) and `Game State Service` (3008). 
- `Game Service` is responsible for core game logic, turn management, and state transitions.
- `Game State Service` is responsible for distributed state management, snapshot persistence, and state validation.

## Decision
We will maintain a strict separation between Game Logic and Game State.
- The **Game Service** will be treated as a stateless "engine" that receives events, fetches the current state from the Game State Service, applies logic, and pushes the updated state back.
- The **Game State Service** will be the "Single Source of Truth" for the current game snapshot, utilizing Redis for high-performance "hot" state and PostgreSQL for long-term persistence/recovery.

## Consequences
### Positive
- **Scalability**: Game Services can be scaled horizontally without worrying about local state synchronization.
- **Resilience**: If a Game Service instance fails during a transition, another instance can pick up the task from the last known state in the Game State Service.
- **Observability**: Centralizing state in one service makes it easier to implement game replays and real-time monitoring.

### Negative
- **Latency**: Every logic transition requires a network round-trip to the Game State Service.
- **Complexity**: Developers must manage the interaction between two services for even simple game operations.
- **Consistency**: Distributed state management requires careful implementation of locking or optimistic concurrency to avoid race conditions.
