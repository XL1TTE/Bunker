# ADR 0002: Pass-through Real-time Broadcaster

## Context
The system needs to broadcast events to clients in real-time. The architecture includes a `Real-time Service` (3006) using Socket.io and a Message Broker (RabbitMQ/Kafka).

## Decision
The `Real-time Service` will be a simple "Pass-through Broadcaster".
- It will NOT perform data sanitization or complex business logic.
- It will subscribe to Message Broker topics and forward payloads directly to the corresponding Socket.io rooms or individual user channels.
- Business services (Game, Lobby, etc.) are responsible for publishing pre-sanitized events.

## Consequences
### Positive
- **Simplicity**: The Real-time Service remains lightweight and generic.
- **Consistency**: Business logic (what to hide/show) is centralized in the domain services rather than split across layers.
- **Performance**: Minimal processing overhead in the socket gateway.

### Negative
- **Broker Noise**: The Message Broker will see a higher volume of messages, including per-user private events.
- **Service Complexity**: Domain services must now manage the logic of "who sees what" when publishing events.
- **Security Risk**: If a domain service accidentally publishes secret data to a public topic, the Real-time Service will blindly broadcast it.
