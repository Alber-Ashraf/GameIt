Last updated: 2026-10-29

# GameIt — Digital PC Game Storefront (.NET 8 Clean Architecture & Blazor WebAssembly)

GameIt is a full-stack PC digital game distribution storefront for discovering, reviewing, and purchasing video games, built on **.NET 8 Clean Architecture**, **CQRS with MediatR**, **ASP.NET Core Identity (JWT)**, **Entity Framework Core**, **Stripe Checkout**, **Hangfire**, and **Blazor WebAssembly**.

---

## Key Features

### Game Management
- Create, update, and delete games (`Administrator` role)
- View game details with category, community rating, active discount, and publisher
- Filter by category, search by title, sort by price/rating, and view featured & similar games

### Category & Discount Management
- Full CRUD for game categories and promotional discount campaigns
- Retrieve categories by name or ID and filter catalog titles by genre
- Automatic background cleanup of expired discounts via Hangfire

### Review System
- Add, update, and delete game reviews with star ratings and comments
- Ownership and `Administrator` role enforcement on review mutations

### Wishlist & Personal Library
- Add, remove, and clear saved games in the personal wishlist
- Automatic provisioning of purchased and free games into the user's downloadable library

### Payments & Purchases
- Server-side authoritative pricing and active discount calculation
- Stripe Checkout integration with webhook verification (`checkout.session.completed`)
- Complete transaction receipt history

### Blazor WebAssembly Storefront (`Core -> Shared -> Features`)
- 3-tier CSS token design system (`tokens.css` + `app.css`)
- Vertical feature slices (`Auth`, `Catalog`, `Categories`, `Discounts`, `Reviews`, `Wishlist`, `Library`, `Purchases`, `Admin`)
- Full 4-state UX coverage (`Loading Skeleton`, `Empty`, `Error`, `Populated`) on every view

---

## Tech Stack

| Category | Tools / Frameworks |
|---|---|
| **Language & Runtime** | C# 12, .NET 8 |
| **Backend** | ASP.NET Core Web API, MediatR, FluentValidation, AutoMapper |
| **Frontend** | Standalone Blazor WebAssembly, `Blazored.LocalStorage` |
| **Shared Contracts** | `GameIt.Contracts` (Strongly-typed Request/Response DTOs) |
| **Database** | SQL Server, Entity Framework Core 8 |
| **Authentication** | JWT Bearer, ASP.NET Core Identity |
| **Payments & Jobs** | Stripe Checkout + Webhooks, Hangfire, SendGrid |
| **Testing** | xUnit, Moq, Shouldly, EF Core InMemory (22/22 unit & integration tests) |

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
│       ├── 0001-clean-architecture-and-contracts.md
│       ├── 0002-blazor-wasm-core-shared-features.md
│       └── 0003-server-side-pricing-and-security-hardening.md
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

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/Alber-Ashraf/GameIt.git
   cd GameIt
   ```
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
- [07 — Deployment (MonsterASP.NET & Docker) Runbook](docs/07-deployment.md)
- [08 — Operations, Observability & Maintenance Runbook](docs/08-operations.md)
- [09 — Contributing Guide & Engineering Standards](docs/09-contributing.md)
- [10 — Execution Roadmap & Completed Milestones](docs/10-roadmap.md)
- **Architecture Decision Records (`docs/adr/`):**
  - [ADR-0001: Clean Architecture & Shared Contracts](docs/adr/0001-clean-architecture-and-contracts.md)
  - [ADR-0002: Blazor WASM Core, Shared & Vertical Features](docs/adr/0002-blazor-wasm-core-shared-features.md)
  - [ADR-0003: Server-Side Pricing & Security Hardening](docs/adr/0003-server-side-pricing-and-security-hardening.md)

---

## Screenshots

<img width="1800" height="202" alt="image" src="https://github.com/user-attachments/assets/1aa732cd-2688-41f5-9046-e14a95be7b1b" />
<img width="1802" height="475" alt="image" src="https://github.com/user-attachments/assets/809d4dc4-fd28-435c-b81c-4194ff3154f4" />
<img width="1805" height="332" alt="image" src="https://github.com/user-attachments/assets/c9692fd2-f13c-4d98-817c-d3e833a2524d" />
<img width="1801" height="611" alt="image" src="https://github.com/user-attachments/assets/8d7b002a-684f-4a1c-bfd1-01eb83e43fb9" />
<img width="1788" height="128" alt="image" src="https://github.com/user-attachments/assets/115a098b-a972-468c-a094-3d5dc60290cd" />
<img width="1802" height="194" alt="image" src="https://github.com/user-attachments/assets/8a0cfa16-4841-45e3-8a04-6eafc754537d" />
<img width="1795" height="337" alt="image" src="https://github.com/user-attachments/assets/61b26cb1-0b00-4db0-8d62-66287ebba19a" />
<img width="1799" height="128" alt="image" src="https://github.com/user-attachments/assets/caba72ca-c1b8-4c41-8767-c8be1df6d056" />
<img width="1805" height="333" alt="image" src="https://github.com/user-attachments/assets/44d8253f-cc9b-4100-ade1-621dcb7f613d" />

---

## Author
Developed by Alber Ashraf
