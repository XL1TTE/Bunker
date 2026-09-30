# Bunker

> Multiplayer survival social deduction card game built with .NET 10 microservices, orchestrated with .NET Aspire.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![.NET Aspire](https://img.shields.io/badge/Aspire-Orchestrated-512BD4)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![Keycloak](https://img.shields.io/badge/Auth-Keycloak-008B8B?logo=redhat)](https://www.keycloak.org/)
[![RabbitMQ](https://img.shields.io/badge/Messaging-RabbitMQ-FF6600?logo=rabbitmq)](https://www.rabbitmq.com/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-336791?logo=postgresql)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## Overview

**Bunker** is an online multiplayer adaptation of the tabletop post-apocalyptic survival game. Players gather in a lobby, receive randomized character sheets (profession, biology, health status, baggage, hobbies, special traits), and argue their value to secure a spot inside the bunker across successive voting and elimination rounds.

---

## Architecture

The backend follows an event-driven microservices architecture orchestrated with **.NET Aspire**:

### Services

| Service | Responsibility |
|---|---|
| **`AppHost`** | .NET Aspire orchestrator bootstrapping all databases, messaging, auth, and backend services. |
| **`Bunker.AccountService`** | Player profiles, synced automatically from Keycloak identity on first authentication. |
| **`Bunker.LobbyService`** | Lobby lifecycle, player readiness, host migration, and game handoff. Maintains a local read-model cache (`lobby-accounts-cache`) of account details. |
| **`Bunker.ContentService`** | Catalog of card packs, character traits, professions, disaster scenarios, and personality presets. Includes `Bunker.ContentService.Defaults` for initial seeding. |
| **`Bunker.GameService`** | Active match state machine: character sheet assignment, rounds, card reveals, and voting/elimination. |
| **`Bunker.Provisioner.RabbitMq`** | Startup worker that declares exchanges, queues, and topologies prior to microservice launch. |
| **`Bunker.Api.Common`** | Cross-cutting concerns: `IUserIdentityContext`, Keycloak JWT handling, middleware, and common DTOs. |
| **`Bunker.Monads`** | Functional `Result<T>` error-handling primitives. |

---

## Tech Stack & Conventions

- **Platform:** .NET 10 / C# 13
- **Orchestration:** [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
- **Messaging & CQRS:** [WolverineFx](https://wolverine.netlify.app/) (durable outbox pattern over RabbitMQ)
- **Authentication:** [Keycloak](https://www.keycloak.org/) (OIDC / OAuth 2.0 JWT Bearer tokens)
- **Data Persistence:** PostgreSQL (dedicated database per bounded context)
- **Mapping:** [Mapperly](https://mapperly.riok.org/) (source-generated compile-time object mapping)
- **Validation:** [FluentValidation](https://docs.fluentvalidation.net/)
- **Error Handling:** Functional `Result<T>` monad pattern (avoiding exception control flow across layers)
- **API Documentation:** [Scalar](https://scalar.com/) (modern OpenAPI document viewer)

---

## Solution Structure

```
Bunker/
├── src/
│   ├── AppHost/                              # .NET Aspire orchestration host
│   ├── Provisioners/
│   │   └── Bunker.Provisioner.RabbitMq/      # RabbitMQ exchange & queue provisioning
│   ├── Bunker.AccountService/                # Player profile management
│   ├── Bunker.ContentService/                # Card, pack, and scenario admin service
│   ├── Bunker.ContentService.Defaults/       # Seed data and pack templates
│   ├── Bunker.LobbyService/                  # Lobby management & account cache
│   ├── Bunker.GameService/                   # Active game session engine
│   └── Shared/
│       ├── Bunker.Api.Common/                # Auth, middlewares, and common contracts
│       └── Bunker.Monads/                    # Functional Result<T> types
├── AGENTS.md                                 # Architecture rules & agent guidelines
└── Bunker.slnx                               # Solution file
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/) or Podman (required by Aspire for containerized dependencies)
- Supported IDE: Visual Studio 2026+ / JetBrains Rider / VS Code with C# Dev Kit

---

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/XL1TTE/Bunker.git
   cd Bunker
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore Bunker.slnx
   ```

3. **Launch the Aspire AppHost:**
   ```bash
   dotnet run --project src/AppHost/AppHost.csproj
   ```

4. **Access the Aspire Dashboard:**
   Open the Aspire dashboard URL printed in the terminal console (e.g., `https://localhost:17123`) to view all running services, container logs, traces, and metrics.

---

## License

This project is licensed under the [MIT License](LICENSE).
