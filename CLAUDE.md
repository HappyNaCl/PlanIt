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
- `Entity<TId>` — ID-based equality, soft delete support (`IsDeleted`, `DeletedAt`), `CreatedAt`/`UpdatedAt` timestamps
- `AggregateRoot<TId>` — extends Entity, marks aggregate transaction boundaries
- `ValueObject` — sequence-based equality for immutable value types

Entities: `User`, `Schedule`, `Attraction`, `Registrant` (junction aggregate for User↔Attraction).

**PlanIt.Application** — Business logic and interfaces. Defines contracts that Infrastructure implements:
- `IAccessTokenGenerator`, `IRefreshTokenGenerator`, `IPasswordHasher`
- `IDatetimeProvider`, `IFileUploader`
- `IApplicationDbContext`, `IUserRepository`

Uses MediatR for CQRS (commands under `Authentication/Commands/`, queries under `Authentication/Queries/`). FluentValidation validators run via `ValidationBehavior<TRequest, TResponse>` pipeline behavior.

**PlanIt.Infrastructure** — Implements Application interfaces. Service registration via `DependencyInjection.cs` with `IOptions<T>` config binding:
- JWT: `AccessTokenGenerator` / `RefreshTokenGenerator` (HS256, `JwtSettings`)
- Passwords: `BCryptPasswordHasher`
- DB: `ApplicationDbContext` (EF Core + Npgsql) with soft-delete query filters and automatic timestamp management
- Repositories: `UserRepository` wrapped by `CachedUserRepository` (decorator pattern, Redis)
- File storage: `S3Uploader` (stub, `S3Settings`)

**PlanIt.Api** — ASP.NET Core entry point. Calls `AddApplication()` and `AddInfrastructure(config)`. Uses Mapster for request→command and result→response mapping. `ExceptionHandlingMiddleware` converts domain exceptions to structured JSON responses.

**PlanIt.Contracts** — C# `record` types for API request/response DTOs.

## Key Infrastructure

| Concern | Technology |
|---|---|
| Database | PostgreSQL via EF Core 9 (Npgsql) |
| Auth | JWT (HS256): 15-min access / 7-day refresh tokens |
| Password hashing | BCrypt.Net-Next |
| Caching | Redis (StackExchange), 10-min TTL, decorator on `IUserRepository` |
| Object storage | S3-compatible (MinIO SDK / RustFS in docker-compose) |
| Mapping | Mapster 10 |
| Messaging | RabbitMQ (client installed, not yet integrated) |
| Real-time | SignalR (installed, not yet integrated) |

## Configuration

All dev settings live in `appsettings.Development.json`:
- `ConnectionStrings:DefaultConnection` — PostgreSQL
- `ConnectionStrings:Redis` — Redis
- `JwtSettings` — Issuer, Audience, AccessTokenSecret, RefreshTokenSecret, AccessExpiryMinutes (15), RefreshExpiryMinutes (25200)
- `S3` — Endpoint, AccessKey, SecretKey, BucketName

Production secrets must be supplied via environment variables or secrets management — never commit real secrets.

## Docker

`docker-compose.yaml` spins up: PostgreSQL (5432), Redis (6379), RustFS/S3 (9000/9001), RabbitMQ (5672/15672). Default credentials are `planit/planit`.