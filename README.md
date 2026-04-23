
# Mechanic Shop Management System

A modern, modular mechanic shop management system built with .NET 9 and Blazor WebAssembly (hosted).
This repository follows a layered architecture and includes observability, API documentation, and containerization support.

## Key Features
- Blazor WebAssembly client (hosted) for a responsive single-page UI
- ASP.NET Core Web API serving client assets
- Clean layered architecture: Client, Api, Application, Infrastructure, Contracts
- EF Core for data access and migrations
- API Versioning and OpenAPI (Swagger)
- Observability: OpenTelemetry with Prometheus exporter
- Docker support for local development and production deployments

## Projects
- src/MechanicShop.Client — Blazor WebAssembly client
- src/MechanicShop.Api — ASP.NET Core backend (hosts the client)
- src/MechanicShop.Application — Business logic and DTOs
- src/MechanicShop.Infrastructure — EF Core and persistence
- src/MechanicShop.Contracts — Shared contracts and DTOs

## Tech Stack
- .NET 9
- Blazor WebAssembly (hosted)
- Entity Framework Core
- OpenTelemetry + Prometheus
- Swashbuckle (Swagger)
- Docker for containerization

## Prerequisites
- .NET 9 SDK
- Docker (for container workflows)
- A SQL-compatible database (SQL Server, PostgreSQL, etc.)

## Quick start — build and run locally
1. Clone the repo

   git clone https://github.com/ahmedsaleh-ctrlz/MechanicShopManagementSystem.git
   cd "Mechanic Shop Management System"

2. Restore and build

   dotnet restore
   dotnet build

3. Configure the database

   Update the connection string in src/MechanicShop.Api/appsettings.Development.json (or secrets) under `ConnectionStrings:DefaultConnection`.

4. Run the hosted app

   dotnet run --project src/MechanicShop.Api

   The API will serve the Blazor client; check the console for the listening URL. Open /swagger for API docs.

## Database migrations
Add or apply EF Core migrations from the repo root:

dotnet ef migrations add InitialCreate --project src/MechanicShop.Infrastructure --startup-project src/MechanicShop.Api --output-dir Migrations
dotnet ef database update --project src/MechanicShop.Infrastructure --startup-project src/MechanicShop.Api

## Docker
This repository includes a Dockerfile for the API which also serves the Blazor client. Two common ways to run in Docker:

1) Build and run image locally

   docker build -t mechanicshop/api:latest -f src/MechanicShop.Api/Dockerfile .
   docker run -e ASPNETCORE_ENVIRONMENT=Production -p 5000:80 mechanicshop/api:latest


Then run:

   docker-compose up --build

## Observability
The API includes OpenTelemetry instrumentation and a Prometheus exporter. Configure exporters and scraping endpoints in `appsettings` as needed and add Prometheus/Grafana to your monitoring stack.

## Contributing
- Fork and create a branch for features/fixes
- Add tests for new behavior
- Open a Pull Request with clear description

## License
MIT — see LICENSE file or add one to the repository.

---

