# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build PlanIt.sln

# Run the API (HTTP: 5254, HTTPS: 7174)
dotnet run --project PlanIt.Api

# Hot-reload development
dotnet watch --project PlanIt.Api

# Restore dependencies
dotnet restore
```

Swagger UI is available at `/swagger` when running in Development mode.

## Architecture

PlanIt follows **Clean Architecture** with **Domain-Driven Design**. Dependencies flow inward:

```
PlanIt.Api → PlanIt.Application → PlanIt.Domain
PlanIt.Infrastructure → PlanIt.Application
PlanIt.Contracts (DTOs, referenced by Api)
```

**PlanIt.Domain** — DDD core: no external dependencies except MediatR. Contains base types:
- `Entity<TId>` — ID-based equality
- `AggregateRoot<TId>` — extends Entity for aggregate roots
- `ValueObject` — sequence-based equality for immutable value types

**PlanIt.Application** — Business logic and interfaces. Defines contracts (e.g., `IAccessTokenGenerator`, `IRefreshTokenGenerator`, `IDatetimeProvider`) that Infrastructure implements. Uses MediatR for CQRS.

**PlanIt.Infrastructure** — Implements Application interfaces. Houses JWT token generation, EF Core/PostgreSQL, Redis, Minio, RabbitMQ, SignalR, BCrypt. Service registration via `DependencyInjection.cs` extension methods with `IOptions<T>` config binding (e.g., `JwtSettings`).

**PlanIt.Api** — ASP.NET Core entry point. Calls `AddApplicationServices()` and `AddInfrastructureServices()` from the respective DI extension methods.

**PlanIt.Contracts** — C# `record` types for API request/response DTOs. Kept separate to allow sharing with clients.

## Key Infrastructure

| Concern | Technology |
|---|---|
| Database | PostgreSQL via EF Core 9 (Npgsql) |
| Auth | JWT (HS256): 15-min access / ~17.5-day refresh tokens |
| Password hashing | BCrypt.Net-Next |
| Caching | Redis (StackExchange) |
| Object storage | Minio (S3-compatible) |
| Messaging | RabbitMQ |
| Real-time | SignalR |

## Configuration

JWT settings live in `appsettings.Development.json` under the `JwtSettings` key. The `JwtSettings` class in Infrastructure binds to this section. Production secrets must be supplied via environment variables or secrets management — never commit real secrets.
