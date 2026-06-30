# Bunker — Agent Guide

## Repo at a glance

.NET 10 / C# 13. Aspire-orchestrated microservices. WolverineFx does in-process
command dispatch and RabbitMQ messaging. PostgreSQL per service. Mapperly
generates mappers at build time. Scalar (not Swagger) for API docs.

## Project map

```
src/
  AppHost/                                 Aspire orchestrator (full-stack entrypoint)
  Bunker.AccountService/                   Web API — player profiles, account-updates
  Bunker.LobbyService/                     Web API — lobbies, lobby-accounts replica
  Bunker.ContentService/                   Web API — cards/packs/personalities (admin)
  Bunker.ContentService.Defaults/          Shared Aspire defaults (OTel, Sentry, health)
  Bunker.Api.Common/                       Shared lib — JWT auth, IUserIdentityContext
  Bunker.Monads/                           Pure Result<T> (zero deps)
  Provisioners/Bunker.Provisioner.RabbitMq/  Declares RabbitMQ fanout exchanges
docs/
  CONTEXT.md                               Domain glossary — read first
```

## Developer commands

```powershell
dotnet build Bunker.slnx                                   # Build all
dotnet run --project src/AppHost                           # Run full stack (Aspire dashboard)
dotnet run --project src/Bunker.AccountService             # Run one service standalone
dotnet publish Bunker.slnx                                 # Publish all
```

Solution is `.slnx`; VS Code uses `dotnet.defaultSolution: Bunker.slnx`, so a
naked `dotnet build` resolves it.

## No tests, CI, linter, or Dockerfiles

There is **no test project, no CI workflow, no linter/formatter config, no
Dockerfile**. `dotnet test` does nothing. Don't waste time looking for them.

## Architecture facts that bite

- **WolverineFx** is the only message bus (`WolverineFx.{RabbitMQ,Postgresql,
  EntityFrameworkCore,RuntimeCompilation}`). Handlers are wired via conventions;
  outbox is durable (`UseDurableOutbox`).
- **RabbitMQ exchanges** are declared in
  `src/Provisioners/Bunker.Provisioner.RabbitMq/ContentProvision.cs` as
  fanouts. The provisioner project **must** finish before any service starts
  (`WaitForCompletion(rabbitMqProvisioner)` in `AppHost.cs`). Exchanges:
  `account-updates`, `sex-card-updates`, `profession-card-updates`,
  `fact-card-updates`, `age-card-updates`, `hobbies-card-updates`.
- **Five PostgreSQL databases**, each its own connection string:
  `game-state-db`, `account-db`, `lobby-db`, `lobby-accounts-replica-db`,
  `content-service-db`. The Lobby service reads from a **replica** of the
  account DB (`lobby-accounts-replica-db`) — don't write to it from Lobby.
- **Database schema is bootstrapped via `EnsureCreatedAsync()`** in each
  service's `PersistenceConfiguration.InitializeDatabaseAsync`, called only in
  `IsDevelopment()`. There are no EF migrations — adding `IEntityTypeConfiguration`
  is enough; creating a new entity type requires no migration step.
- **Aspire** provisions infra: PostgreSQL (each db has its own server +
  `WithPgAdmin()`), Keycloak, RabbitMQ (with management plugin), Redis, Python.
  AppHost reads secret params via `builder.AddParameter(..., secret: true)` and
  resolves them from user-secrets at run time.
- **Identity** is Keycloak with JWT Bearer. User id flows through `IUserIdentityContext`
  (scoped) populated by `UserIdentityMiddleware`; `PlayerProfileSyncMiddleware`
  on AccountService lazy-creates the profile on first auth.
- **Mapperly** generates mapping code (`.g.cs`) at build time. Mappers live in
  `Persistence/Mappers/` and `Transfers/Mappers/`. Decorated with `[Mapper]`
  from `Riok.Mapperly.Abstractions`.
- **`Bunker.ContentService.Defaults`** provides `AddServiceDefaults()` — wired
  into ContentService only. Other services have their own thin `ConfigureLogging`
  / Sentry setup. When adding a new service, copy whichever sibling matches.
- **Api docs**: Scalar at `/scalar` in Development for each service; OpenAPI
  documents every endpoint (including admin-only content management).
- **`.http` request files** exist next to each service (`Bunker.AccountService.http`,
  `Bunker.ContentService.http`, `LobbyService.http`) — they reference the
  default ports from `Properties/launchSettings.json` (e.g. `http://localhost:5165`
  for Account). Note: the AccountService `.http` still references a leftover
  `/weatherforecast/` endpoint that no longer exists in the project — delete
  it when rewriting.

## Configuration & secrets

`AppHost` and each service have their own `UserSecretsId` in the `.csproj`.
Parameters AppHost needs at run time:

```powershell
# AppHost secrets (only required when running through Aspire)
dotnet user secrets set "Parameters:rabbitmq-user" "..."
dotnet user secrets set "Parameters:rabbitmq-password" "..."
dotnet user secrets set "Parameters:keycloak-user" "..."
dotnet user secrets set "Parameters:keycloak-password" "..."
dotnet user secrets set "Parameters:game-state-db-user" / -password
dotnet user secrets set "Parameters:account-db-user" / -password
dotnet user secrets set "Parameters:lobby-db-user" / -password
dotnet user secrets set "Parameters:content-service-db-user" / -password
# Sentry for ContentService (uses Sentry:Dsn)
dotnet user secrets set "Sentry:Dsn" "..."
```

When running a single service outside AppHost, the Aspire-injected connection
strings/env vars won't be present — either spin up the dependency stack
yourself or run through AppHost.

## Conventions

- C# **extension blocks** (C# 13 `extension(SomeType receiver)`) are used
  everywhere for `IServiceCollection`/`IHostApplicationBuilder`/`WebApplication`
  extensions. Keep that style; don't add `this` parameters.
- Persistence layer: `DbContext` + `IUnitOfWork` registration per service.
  Wolverine EFCore transactions are enabled with
  `UseEntityFrameworkCoreTransactions().WithDbContextAbstraction<IUnitOfWork, TContext>()`.
- Result types: prefer `Bunker.Monads.Result<T>` over throwing across layers.
- FluentValidation for request DTOs; validators in `Validation/`.
- No comments unless asked.

## Domain language

Read `docs/CONTEXT.md` first. Key terms: Game, Player, Player Profile,
Keycloak-Linked Identity, Lobby (readiness, host migration, grace period),
Bunker cards (sex/profession/fact/age/hobbies), Character Sheets, Attributes,
Elimination, Survival, Card Packs, Personality Presets, Lobby Handoff saga.
