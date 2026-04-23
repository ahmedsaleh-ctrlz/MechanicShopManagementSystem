# Mechanic Shop Management System

A full-stack workshop operations platform built with **.NET 9**, **ASP.NET Core**, and **hosted Blazor WebAssembly**.
It helps teams run day-to-day service work: customer intake, repair catalog management, scheduling, work order tracking, and billing.

## Features

- JWT-based authentication with role-aware authorization (`Manager`, `Labor`)
- Customer management with multi-vehicle support
- Repair task catalog with labor cost, duration, and parts
- Work order lifecycle management:
  - create, search, filter, sort, and paginate
  - assign labor and repair tasks
  - relocate across time/spot
  - transition state (`Pending`, `InProgress`, etc.)
- Daily schedule view by bay/spot and by labor
- Dashboard stats endpoint for daily operations
- Invoice issuing, settlement, and PDF export
- Real-time work-order refresh with SignalR
- API versioning + OpenAPI docs (Swagger + Scalar)
- Rate limiting, output caching, global error handling, and structured logging
- Observability with OpenTelemetry, Prometheus exporter, Seq, and Grafana (via Docker Compose)
- Seeded development data for instant local usage

## Architecture

The solution follows a clean, layered structure:

- `src/MechanicShop.Api`: ASP.NET Core host, controllers, middleware, OpenAPI, SignalR hub, and Blazor hosting
- `src/MechanicShop.Client`: Blazor WebAssembly UI
- `src/MechanicShop.Application`: use cases (commands/queries), orchestration, and DTO contracts
- `src/MechanicShop.Domain`: core domain entities and business rules
- `src/MechanicShop.Infrastructure`: EF Core, Identity/JWT, policies, persistence, background jobs, and integrations
- `src/MechanicShop.Contracts`: request/response contracts shared across layers
- `tests/*`: unit and test-support projects

## Core Business Modules

- **Identity**: token generation, refresh tokens, current-user claims
- **Customers**: CRUD with embedded vehicle records
- **Repair Tasks**: predefined service catalog with required parts
- **Work Orders**: planning, assignment, state transitions, and relocation
- **Scheduling**: day-level bay/labor schedule retrieval with timezone support
- **Billing**: issue invoice from work order, mark paid, download PDF
- **Dashboard**: daily operational stats

## Prerequisites

- .NET SDK 9.0+
- SQL Server (local or containerized)
- Docker Desktop (optional, for full container stack)

## Quick Start (Local Development)

1. Clone the repository

```bash
git clone https://github.com/ahmedsaleh-ctrlz/MechanicShopManagementSystem.git
cd "Mechanic Shop Management System"
```

2. Configure connection string (if needed)

Default key:

`src/MechanicShop.Api/appsettings.json` -> `ConnectionStrings:DefaultConnection`

3. Restore, build, and run

```bash
dotnet restore
dotnet build
dotnet run --project src/MechanicShop.Api
```

4. Open the app and API docs

- App/API host (default dev profile): `https://localhost:7275` or `http://localhost:5072`
- Swagger UI: `https://localhost:7275/swagger`
- OpenAPI JSON: `https://localhost:7275/openapi/v1.json`
- Scalar API reference (development): `https://localhost:7275/scalar`

## Seeded Demo Users (Development)

The app seeds roles, users, and sample workshop data in development.

- Manager:
  - Email: `pm@localhost`
  - Password: `pm@localhost`
- Labor users:
  - `john.labor@localhost`
  - `peter.labor@localhost`
  - `kevin.labor@localhost`
  - `suzan.labor@localhost`
  - Password for each: same as email

Sample customers, vehicles, repair tasks, employees, and work orders are also created automatically.

## API Usage Notes

- Most API routes are under `api/v1/...`
- Include bearer token in `Authorization: Bearer <token>`
- Daily schedule endpoint expects timezone header:
  - `X-TimeZone: <IANA or Windows timezone id>`

## Docker / Container Stack

Run the full stack (API + SQL Server + Seq + Prometheus + Grafana):

```bash
docker compose up --build
```

Default service ports:

- API: `http://localhost:5001`
- SQL Server: `localhost:1433`
- Seq: `http://localhost:8081` (ingest on `:5341`)
- Prometheus: `http://localhost:9090`
- Grafana: `http://localhost:3000`

## Database & Migrations

From repository root:

```bash
dotnet ef migrations add <MigrationName> --project src/MechanicShop.Infrastructure --startup-project src/MechanicShop.Api
dotnet ef database update --project src/MechanicShop.Infrastructure --startup-project src/MechanicShop.Api
```

## Testing

Run all tests:

```bash
dotnet test
```

Current test focus:

- `MechanicShop.Domain.UnitTests`: domain behavior and invariants
- `MechanicShop.Tests.Common`: shared test factories and utilities
- Additional test projects are included for application and API-level coverage

CI pipeline (`.github/workflows/build-and-test.yml`) runs:

- `dotnet restore`
- `dotnet build --configuration Release --no-restore`
- `dotnet test --configuration Release --no-build`

## Contributing

1. Create a feature branch
2. Keep changes scoped and tested
3. Run `dotnet build` and `dotnet test` before pushing
4. Open a pull request with context, screenshots (if UI), and test notes

## License

MIT
