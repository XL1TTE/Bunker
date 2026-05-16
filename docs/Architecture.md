# Implementation Plan: Tech Stack, Ecosystem, and Architecture

## 1. Background & Motivation
The Bunker game is a distributed, real-time multiplayer game relying on complex social mechanics, state management, and LLM-based AI bots. To ensure system resilience, scalability, and maintainability, we are adopting a modern, highly structured .NET 10 ecosystem orchestrated by .NET Aspire, utilizing Wolverine for robust messaging, and adhering to strict Minimal API and Vertical Slice architectural patterns.

## 2. Scope & Impact
This plan defines the concrete technologies, languages, frameworks, and structural patterns to be used for the entire Bunker project. It covers the core microservices, the AI service, real-time communication, data persistence, and the messaging backbone. 

The decisions here align with the domain model established (Group Survival, Bunker Cards, Full Suite Characters), architectural constraints (ADR 0001, ADR 0002), and strict coding standards provided by the user.

## 3. Proposed Solution

### 3.1. Ecosystem & Orchestration
*   **Primary Framework:** **.NET 10 / C#**. 
*   **Solution Format:** Use the new `.slnx` solution format.
*   **Orchestration:** **.NET Aspire**. Manual Dockerfiles and `docker-compose.yml` will NOT be used. Aspire will orchestrate all C# microservices, the Python AI service, PostgreSQL, Redis, RabbitMQ, and Keycloak during local development.

### 3.2. Microservices Breakdown

| Service | Responsibility | Technology Stack |
| :--- | :--- | :--- |
| **Identity Provider** | User authentication, token generation. | **Keycloak** (Replaces custom Auth Service) |
| **Player Service** | Profiles, statistics, persistent user data. | .NET 10 Web API, EF Core |
| **Lobby Service** | Invite codes, Lobby Browser, slot allocation. | .NET 10 Web API, EF Core |
| **Game Service** | Stateless game logic, turn/phase management. | .NET 10 Web API |
| **Game State Service** | Single Source of Truth for active game state. | .NET 10, Redis, EF Core |
| **Matchmaking Service** | Match players/lobbies (Private-first priority). | .NET 10 Worker Service |
| **Real-time Service** | Pass-through WebSocket broadcaster (ADR 0002). | .NET 10 SignalR |
| **AI Bot Service** | LLM-based social persuasion, proposal evaluation. | **Python / FastAPI + LangChain** (Orchestrated via Aspire) |

### 3.3. Structural & API Architectural Patterns
Every .NET microservice MUST adhere to the following strict patterns:

*   **Vertical Slice Architecture:** Code within each service will be organized by feature (e.g., `Features/CreateLobby`, `Features/JoinLobby`) rather than by technical layer (e.g., Controllers, Services, Repositories).
*   **Minimal APIs & Static Handlers:** 
    *   Controllers will NOT be used.
    *   Endpoints must be grouped inside static classes (e.g., `internal static class LobbyEndpoints`).
    *   Each route handler must be a `static` method.
    *   Routing configuration must be separated into extension methods (e.g., `internal static void IncludeLobbies(this IEndpointRouteBuilder builder)`).
*   **Program.cs Structure:** The `Program.cs` file must remain perfectly clean, consisting ONLY of extension method calls (e.g., `builder.IncludePersistence()`, `builder.ConfigureWolverine()`, `app.IncludeLobbies()`). All actual configuration logic must reside in separate static classes inside a `Configuration` folder.
*   **API Documentation:** 
    *   **ScalarApi** will be used for the UI (`app.MapScalarApiReference()`), replacing standard Swagger UI.
    *   OpenAPI schemas will be enriched using XML comments (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`).
*   **Validation:** **FluentValidation** will be used for HTTP request validation. Rules will be automatically exported to the OpenAPI schema using `MicroElements.AspNetCore.OpenApi.FluentValidation`.
*   **Records & Factories:** Extensive use of C# 9+ `record` types with static factory extension methods (e.g., `MyRecordFactory.Create()`) and `ImmutableList<T>`.

### 3.4. Infrastructure, Data & Messaging
*   **Primary Database:** **PostgreSQL** via Entity Framework Core (EF Core).
*   **In-Memory / Hot State:** **Redis** for active game snapshots in the Game State Service.
*   **Messaging & Mediator:** **Wolverine**.
    *   Wolverine will be used for the internal Mediator pattern (CQRS).
    *   Wolverine will be used for external messaging over **RabbitMQ**.
    *   **Reliability:** The **Wolverine Transactional Outbox** pattern will be used in conjunction with EF Core and PostgreSQL to guarantee reliable event delivery even in the event of broker or service crashes.

## 4. Alternatives Considered
*   **Custom Auth Service:** Dropped in favor of Keycloak to offload identity management complexity and adhere to industry standards.
*   **Docker Compose:** Dropped in favor of .NET Aspire, which provides superior developer experience, built-in telemetry, and service discovery out of the box.
*   **MediatR / MassTransit:** Dropped in favor of Wolverine to consolidate internal command dispatching and external event routing under a single, highly performant framework that integrates flawlessly with EF Core Outbox.

## 5. Implementation Plan (Phased)

*   **Phase 1: Aspire & Foundation**
    *   Create the `.slnx` solution.
    *   Initialize the `.AppHost` and `.ServiceDefaults` Aspire projects.
    *   Configure Aspire to spin up PostgreSQL, Redis, RabbitMQ, and Keycloak containers.
    *   Create base libraries for C# Record factories, Wolverine configurations, and Minimal API exception middlewares.
*   **Phase 2: Core Domain Services**
    *   Implement Player Service and Lobby Service using Vertical Slices and static Minimal API handlers.
    *   Configure Keycloak JWT Bearer validation.
    *   Implement the SignalR Real-time Pass-through gateway.
*   **Phase 3: Game Engine & State**
    *   Implement the Game State Service (Redis + PostgreSQL Checkpoints).
    *   Implement the Game Service logic, utilizing Wolverine Outbox to publish state transition events reliably.
*   **Phase 4: AI Integration**
    *   Build the Python FastAPI AI Bot Service.
    *   Integrate the Python service into the `.AppHost` orchestration.
    *   Connect the AI service to RabbitMQ to consume domain events.

## 6. Verification
*   **Unit Tests:** Verify static Minimal API handlers and Record factory validation rules.
*   **Integration Tests:** Verify Wolverine Outbox successfully commits messages to PostgreSQL and forwards them to RabbitMQ.
*   **End-to-End Prototype:** Boot the entire suite via `dotnet run` on the Aspire AppHost. Authenticate via Keycloak, create a lobby via Scalar API, trigger an event that propagates through Wolverine to RabbitMQ, and verify the Python AI service receives it.
