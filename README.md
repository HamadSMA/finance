# Finance

A personal expense tracking application built as a full-stack learning project: an
ASP.NET Core (.NET 10) backend organised around Clean Architecture, backed by
PostgreSQL, secured with OpenID Connect, and consumed by a React + TypeScript
frontend.

## Tech Stack

| Layer          | Technology                                   |
| -------------- | -------------------------------------------- |
| Backend        | ASP.NET Core 10, C#                          |
| Persistence    | EF Core, PostgreSQL                          |
| Authentication | Keycloak (OIDC / OAuth 2.0 / JWT)            |
| Frontend       | React, TypeScript                            |
| Testing        | Unit + integration tests against PostgreSQL  |
| Infrastructure | Docker, Docker Compose, GitHub Actions       |

## Solution Structure

```text
expense-tracker/
├── Finance.slnx
├── src/
│   ├── Finance.Domain/          # Entities, value objects, domain events - no dependencies
│   ├── Finance.Application/     # Use cases, abstractions, DTOs → Domain
│   ├── Finance.Infrastructure/  # EF Core, persistence, external services → Application
│   └── Finance.Api/             # Controllers, middleware, composition root
└── tests/
```

Dependencies point inward only: `Finance.Domain ← Finance.Application ← Finance.Infrastructure`,
with `Finance.Api` referencing `Application` and `Infrastructure` as the composition root.

## Getting Started

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
dotnet restore
dotnet build
dotnet run --project src/Finance.Api
```

## Roadmap

- [ ] **Phase 1 - Core Backend** · API, Clean Architecture, EF Core, PostgreSQL, Expense CRUD
- [ ] **Phase 2 - Real API** · DTOs, validation, filtering, sorting, pagination, ProblemDetails, dashboard aggregation endpoints
- [ ] **Phase 3 - Security** · Keycloak, OIDC, OAuth 2.0, JWT, policies, ownership
- [ ] **Phase 4 - Frontend** · React, TypeScript, OIDC, expense UI, dashboard, charts, API integration
- [ ] **Phase 5 - Quality** · Unit tests, integration tests, PostgreSQL test database, logging, health checks
- [ ] **Phase 6 - Infrastructure** · Docker, Docker Compose, GitHub Actions, container registry
- [ ] **Phase 7 - Advanced (Optional)** · Azure Container Apps, Entra ID (optional swap replacing Keycloak), Redis, OpenTelemetry, rate limiting, performance optimization

## License
See [LICENSE](LICENSE).
