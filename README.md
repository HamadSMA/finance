# Finance: Personal Expense Tracker

A single-user expense tracker: log what you spent, categorise it, and see where the money went on a dashboard. Built as a portfolio project. The backend is an ASP.NET Core 10 API organised around Clean Architecture, backed by PostgreSQL. Amounts are in SAR; there is no multi-currency support and there are no accounts, budgets, or transfers. The domain is deliberately small so the engineering around it can be the interesting part.

**Status: in active development.** Phase 1 (core backend) is complete and runnable. Phases 2 to 6 are in progress; see [Roadmap](#roadmap) for what is built and what is planned. Sections below describe only what is in the code today, and anything planned is labelled as such.

## Screenshots

None yet. The frontend is a Phase 4 deliverable and has not been started, so there is no UI to show. The API can be exercised with `api-tests/phase1.http` or through the OpenAPI document; see [Running Locally](#running-locally).

## Tech Stack

Built and running:

- C#, .NET 10, ASP.NET Core Web API (controllers)
- EF Core 10 with Npgsql, PostgreSQL, code-first migrations
- OpenAPI via `Microsoft.AspNetCore.OpenApi`
- RFC 7807 ProblemDetails for errors

Planned, not yet in the repository:

- Request validation with DataAnnotations (Phase 2)
- Keycloak for OIDC / OAuth 2.0 / JWT bearer auth (Phase 3)
- React, TypeScript, Vite (Phase 4)
- xUnit and Testcontainers (Phase 5)
- Docker, Docker Compose, GitHub Actions (Phase 6)

## Running Locally

Prerequisites: .NET SDK 10.0 and a PostgreSQL 14+ instance. Nothing else is needed yet; there is no frontend or Compose file to run.

Clone, point the API at a database, apply migrations, and run:

```bash
git clone https://github.com/<your-username>/finance.git
cd finance

export ConnectionStrings__FinanceDb="Host=localhost;Port=5432;Database=finance;Username=postgres;Password=postgres"

dotnet restore
dotnet ef database update --project src/Finance.Infrastructure --startup-project src/Finance.Api
dotnet run --project src/Finance.Api
```

The API listens on http://localhost:5048. Adjust the connection string to match your PostgreSQL user, or create the database first with `createdb finance`.

If you prefer a config file to an environment variable, copy the committed template instead of exporting the variable:

```bash
cp src/Finance.Api/appsettings.Example.json src/Finance.Api/appsettings.json
```

Then edit the `ConnectionStrings:FinanceDb` value. `appsettings.json` is gitignored, so it stays local to you; `appsettings.Example.json` is committed so a fresh clone always has a starting point.

`dotnet ef` is required for the migration step. If you do not have it: `dotnet tool install --global dotnet-ef`.

Applying the migration creates the schema and seeds the nine fixed categories (Food, Transport, Housing, Shopping, Entertainment, Bills, Healthcare, Travel, Other).

Once running:

- `GET http://localhost:5048/api/categories` returns the nine seeded categories
- `GET http://localhost:5048/openapi/v1.json` serves the OpenAPI document in Development
- `api-tests/phase1.http` is a REST Client suite covering the expense and category endpoints end to end, with the expected status code noted next to each request

These steps were verified from a clean state on .NET 10.0.103 against PostgreSQL 14 on 2026-09-08, with no `appsettings.json` present, using only the environment variable above. `dotnet build Finance.slnx` succeeds with no warnings.

## Architecture

Four projects under `src/`, following Clean Architecture. The domain holds entities and the fixed category list and references nothing: no ASP.NET Core, no EF Core, no provider SDKs. Application owns DTOs and the `IFinanceDbContext` abstraction; Infrastructure implements that abstraction with EF Core and Npgsql; the API layer holds controllers and composition. The point of the structure on a domain this small is that persistence and identity are swappable without touching business rules.

```text
API → Application
Infrastructure → Application + Domain
Application → Domain
Domain → nothing
```

## Features

Implemented today:

- Create, read, update, and delete expenses, each with an amount, description, date, and category.
- Read-only category listing over nine fixed categories, seeded by the initial migration. Categories are reference data and have no create, update, or delete endpoints by design.
- RFC 7807 ProblemDetails responses, including a 404 body for a missing expense and a global handler for unhandled exceptions, with no stack traces or database detail leaked.
- Database-level querying with EF Core projections, so list responses select only the response shape rather than loading full entities.
- Schema managed by code-first migrations, with indexes on `UserId`, `ExpenseDate`, and the two combined.

Not yet implemented. These are the next phases, listed so the gap is explicit:

- Request validation. `POST /api/expenses` currently accepts a negative amount, an empty description, and a future date. DataAnnotations rules are planned for Phase 2.
- Filtering, sorting, and pagination. `GET /api/expenses` returns every expense as a plain array; query parameters are ignored and there is no paged envelope. Planned for Phase 2.
- Dashboard aggregation endpoints. Planned for Phase 2.
- Authentication and per-user ownership. Queries are scoped to a hardcoded `DevUser.Id` constant in `Finance.Domain`, which is a development placeholder, not an auth model. Keycloak and JWT bearer validation are planned for Phase 3. **Do not deploy this as-is; there is no access control.**
- Health checks, tests, Docker, and CI. Planned for Phases 5 and 6.

## API Endpoints

Implemented and verified against `src/Finance.Api/Controllers/`:

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/expenses` | List all expenses, newest first. No filtering or paging yet. |
| GET | `/api/expenses/{id}` | Fetch one expense; 404 as ProblemDetails if absent |
| POST | `/api/expenses` | Create an expense; 201 with a Location header |
| PUT | `/api/expenses/{id}` | Update an expense; 204 on success, 404 if absent |
| DELETE | `/api/expenses/{id}` | Delete an expense; 204 on success, 404 if absent |
| GET | `/api/categories` | List the nine fixed categories |

Planned, and not currently routable:

| Method | Route | Phase |
| --- | --- | --- |
| GET | `/api/dashboard/summary` | 2 |
| GET | `/api/dashboard/by-category` | 2 |
| GET | `/api/dashboard/by-day` | 2 |
| GET | `/api/dashboard/top-expenses` | 2 |
| GET | `/health` | 5 |

## Testing

There are no automated tests yet. `tests/` is an empty placeholder and the solution contains no test projects, so `dotnet test` would run nothing. Until Phase 5, the API is exercised manually through `api-tests/phase1.http`.

Planned for Phase 5: xUnit unit tests for validation rules and dashboard calculations, and integration tests driving real HTTP requests through the API down to a PostgreSQL database via Testcontainers, so query, mapping, and migration behaviour is exercised rather than mocked.

## Roadmap

- [x] **Phase 1, Core Backend.** API, Clean Architecture, EF Core, PostgreSQL, expense CRUD, seeded categories, ProblemDetails.
- [ ] **Phase 2, Real API.** DTO validation, filtering, sorting, pagination, dashboard aggregation endpoints.
- [ ] **Phase 3, Security.** Keycloak, OIDC, OAuth 2.0, JWT bearer, policies, per-user ownership replacing `DevUser`.
- [ ] **Phase 4, Frontend.** React, TypeScript, OIDC login, expense UI, dashboard, charts.
- [ ] **Phase 5, Quality.** Unit and integration tests, PostgreSQL test database, structured logging, health checks.
- [ ] **Phase 6, Infrastructure.** Dockerfiles, Docker Compose for API + PostgreSQL + Keycloak + frontend, GitHub Actions CI, container registry.
- [ ] **Phase 7, Optional.** Azure Container Apps, Entra ID swap for Keycloak, Redis, OpenTelemetry, rate limiting.

Once Phase 6 lands, the intended entry point becomes a single `docker compose up` that brings up the API, PostgreSQL, Keycloak, and the frontend together, so the project is clone-and-run with no external tenant to configure. That file does not exist yet, so the backend steps above are the current path.

## License

See [LICENSE](LICENSE).
