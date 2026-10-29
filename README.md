Last updated: 2026-10-29

# GameIt — Digital PC Game Storefront (.NET 8 Clean Architecture & Blazor WebAssembly)

GameIt is an enterprise-grade PC digital game distribution storefront built on **.NET 8 Clean Architecture**, **CQRS with MediatR**, **ASP.NET Core Identity (JWT)**, **Entity Framework Core**, **Stripe Checkout**, **Hangfire**, and **Blazor WebAssembly**.

---

## Solution Architecture

```text
GameIt/
├── Directory.Build.props
├── GameIt.sln
├── Dockerfile.api
├── Dockerfile.blazor
├── docker-compose.yml
├── .env.example
├── .github/workflows/ci-cd.yml
├── docs/
│   ├── 01-requirements.md
│   ├── 02-architecture.md
│   ├── 03-database.md
│   ├── 04-api.md
│   ├── 05-ui-ux.md
│   ├── 06-testing.md
│   ├── 07-deployment.md
│   ├── 08-operations.md
│   ├── 09-contributing.md
│   ├── 10-roadmap.md
│   └── adr/
│       ├── 0001-clean-architecture-and-solution-structure.md
│       ├── 0002-shared-contracts-and-vertical-slices.md
│       └── 0003-jwt-authentication-and-security-hardening.md
├── src/
│   ├── Core/
│   │   ├── GameIt.Domain/             # Pure Domain Entities (zero external dependencies)
│   │   ├── GameIt.Contracts/          # Shared API DTOs, Requests & Responses
│   │   └── GameIt.Application/        # CQRS Commands, Queries, Validators & Interfaces
│   ├── Infrastructure/
│   │   ├── GameIt.Persistence/        # EF Core GameItDbContext & Repositories
│   │   ├── GameIt.Identity/           # ASP.NET Core Identity, JWT AuthService & Seeding
│   │   └── GameIt.Infrastructure/     # Stripe Payment Service, SendGrid Email & Hangfire Jobs
│   └── Presentation/
│       ├── GameIt.API/                # REST Controllers, RFC 7807 Middleware, Rate Limiting & /health
│       └── GameIt.BlazorUI/           # Blazor WebAssembly Storefront (Core -> Shared -> Features)
└── tests/
    ├── GameIt.Application.UnitTests/          # 17 Unit & Security Abuse-Case Tests
    └── GameIt.Persistence.IntegrationTests/   # 5 EF Core & Repository Integration Tests
```

---

## Quick Start

### Option A: Local .NET 8 CLI

1. **Prerequisites:** .NET 8.0 SDK and SQL Server / LocalDB (`(localdb)\mssqllocaldb`).
2. **Configure Secrets:** Copy `.env.example` or configure user secrets in `src/Presentation/GameIt.API`:
   ```bash
   dotnet user-secrets set "JwtSettings:SecretKey" "your-at-least-32-character-secret-key" --project src/Presentation/GameIt.API
   ```
3. **Restore, Build & Test:**
   ```bash
   dotnet restore GameIt.sln
   dotnet build GameIt.sln --configuration Release
   dotnet test GameIt.sln --configuration Release
   ```
4. **Run Backend API & Blazor WebAssembly UI:**
   ```bash
   # Terminal 1 — Start ASP.NET Core Web API (https://localhost:7040 / http://localhost:5161)
   dotnet run --project src/Presentation/GameIt.API --launch-profile https

   # Terminal 2 — Start Blazor WebAssembly Storefront (https://localhost:7294 / http://localhost:5036)
   dotnet run --project src/Presentation/GameIt.BlazorUI --launch-profile https
   ```

### Option B: Docker Compose

```bash
cp .env.example .env
docker compose up --build -d
```
- **Blazor Storefront:** `http://localhost:5036`
- **Web API & Health Probe:** `http://localhost:5161/health`

---

## Documentation Index

- [01 — Requirements, Gap Matrix & Security Audit](docs/01-requirements.md)
- [02 — Target Clean Architecture & Solution Structure](docs/02-architecture.md)
- [03 — Database Schema, ER Diagram & Persistence](docs/03-database.md)
- [04 — REST API Specification & Error Contract](docs/04-api.md)
- [05 — Enterprise UI/UX Design System & Component Kit](docs/05-ui-ux.md)
- [06 — Testing Strategy, Security Verification & Web Performance QA](docs/06-testing.md)
- [07 — Deployment, Containerization & Release Runbook](docs/07-deployment.md)
- [08 — Operations, Observability & Maintenance Runbook](docs/08-operations.md)
- [09 — Contributing Guide & Engineering Standards](docs/09-contributing.md)
- [10 — Execution Roadmap & Completed Milestones](docs/10-roadmap.md)
- **Architecture Decision Records (`docs/adr/`):**
  - [ADR-0001: Clean Architecture & Flattened `src/`/`tests/` Structure](docs/adr/0001-clean-architecture-and-solution-structure.md)
  - [ADR-0002: Shared `GameIt.Contracts` & Blazor Vertical Slices](docs/adr/0002-shared-contracts-and-vertical-slices.md)
  - [ADR-0003: JWT Security Hardening & Server-Authoritative Pricing](docs/adr/0003-jwt-authentication-and-security-hardening.md)
