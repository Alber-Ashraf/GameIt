Last updated: 2026-10-29

<h1 align="center">🎮 GameIt</h1>
<p align="center">
  <b>Enterprise Digital PC Game Storefront</b>
  <br />
  Built with <b>.NET 8 Clean Architecture</b>, <b>CQRS with MediatR</b>, <b>Blazor WebAssembly</b>, and <b>Stripe Payments</b>.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/C%23-12-239120?logo=csharp&logoColor=white" alt="C# 12" />
  <img src="https://img.shields.io/badge/Blazor-WebAssembly-512BD4?logo=blazor&logoColor=white" alt="Blazor WebAssembly" />
  <img src="https://img.shields.io/badge/EF%20Core-8.0-512BD4" alt="EF Core 8" />
  <img src="https://img.shields.io/badge/SQL%20Server-2022-CC292B?logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
  <img src="https://img.shields.io/badge/Stripe-Checkout-635BFF?logo=stripe&logoColor=white" alt="Stripe" />
  <img src="https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white" alt="Docker" />
  <img src="https://img.shields.io/badge/Tests-22%2F22%20Passing-brightgreen?logo=xunit" alt="Tests" />
</p>

---

## 📌 Overview

**GameIt** is a full-stack PC digital game distribution storefront designed with enterprise **Clean Architecture** patterns. It features an ASP.NET Core Web API backend sharing strongly-typed DTO contracts with a standalone Blazor WebAssembly frontend, supported by EF Core 8, ASP.NET Core Identity (JWT), Stripe Checkout, Hangfire background jobs, and a 3-tier CSS token design system.

---

## 🚀 Key Features

### 🕹️ Game Catalog & Discovery
- **Rich Catalog Browsing:** Filter by genre, search titles in real time, and sort by price or community rating.
- **Spotlight & Deals:** Hero carousel featuring top titles, discounted games with countdowns, and similar game recommendations.
- **Game Details:** Full specifications, developer/publisher details, cover art, discount badges, and verified customer reviews.

### 🗂️ Categories & Discounts
- **Genre Taxonomy:** Full category CRUD with automatic title counts and genre-scoped catalog views.
- **Promotional Campaigns:** Percentage-based discount campaigns with active date windows (`CK_Discounts_Percentage`, `CK_Discounts_DateRange`).
- **Automated Expiration:** Background recurring cron jobs (Hangfire) to automatically retire expired discounts.

### ⭐ Community Reviews
- **User Reviews:** Verified rating (1–5 stars) and review text management.
- **Authorization & Security:** Strict ownership and `Administrator` role enforcement on review updates and deletions (IDOR/BOLA protected).

### ❤️ Wishlist & 📚 Personal Library
- **Wishlist Manager:** Save favorite games with instant checkout routing and bulk clear confirmation.
- **Digital Vault (Library):** Downloadable personal library automatically provisioned upon purchase or instant claim of free titles.

### 💳 Payments & Authoritative Pricing
- **Tamper-Proof Checkout:** Authoritative server-side price calculation in `CreatePurchaseCommandHandler`—client-provided prices are never trusted.
- **Stripe Checkout:** Hosted Stripe checkout session creation with secure webhook verification (`checkout.session.completed`).
- **Transaction History:** Detailed purchase history with order status tags (`Completed`, `Pending`, `Refunded`) and receipts.

### 🔐 Identity & Access Control
- **JWT Authentication:** Secure token-based auth with refresh lifecycle, password hashing, and configurable token durations.
- **Role-Based Authorization:** Fine-grained access control separating standard `User` accounts from `Administrator` catalog management.
- **Rate Limiting:** Fixed-window rate limiting (`AuthPolicy`: 10 req/min per IP) on login and registration endpoints.

### 🌐 Blazor WebAssembly Storefront (`Core -> Shared -> Features`)
- **Vertical-Slice Architecture:** Modular feature slices (`Auth`, `Catalog`, `Categories`, `Discounts`, `Reviews`, `Wishlist`, `Library`, `Purchases`, `Admin`).
- **Complete UX Coverage:** Guaranteed 4-state lifecycle on every view: **Loading (Skeleton)**, **Empty**, **Error**, and **Populated**.
- **Design System:** 3-tier CSS token hierarchy (`Primitive -> Semantic -> Component`) with responsive mobile-first navigation.

---

## 🛠️ Tech Stack

| Layer | Technologies & Libraries |
|---|---|
| **Backend Framework** | ASP.NET Core 8 Web API, C# 12 |
| **Architecture** | Clean Architecture, CQRS, MediatR 12, Repository + Unit of Work |
| **Data & Persistence** | Entity Framework Core 8, SQL Server 2022, Code-First Migrations |
| **Validation & Mapping** | FluentValidation 12, AutoMapper 12 |
| **Authentication** | ASP.NET Core Identity, JWT Bearer Tokens, Custom Password Hasher |
| **Payments & Integrations** | Stripe.net (Checkout Sessions & Webhooks), SendGrid, Hangfire |
| **Frontend Framework** | Blazor WebAssembly (.NET 8), `Blazored.LocalStorage` |
| **UI & Styling** | 3-tier CSS Token Design System (`tokens.css` + `app.css`), Responsive Sidebar |
| **Testing** | xUnit, Moq, Shouldly, EF Core InMemory (22/22 unit & integration tests) |
| **DevOps & Hosting** | Docker, Docker Compose, Nginx, MonsterASP.NET (IIS), GitHub Actions CI/CD |

---

## 🔑 Demo Credentials

When the database is created, the system automatically seeds default accounts:

| Role | Email | Password | Access Scope |
|---|---|---|---|
| **Administrator** | `admin@gameit.com` | `P@ssword1` | Catalog CRUD, Categories, Discounts, Admin Dashboard |
| **User** | `user@gameit.com` | `P@ssword1` | Storefront, Wishlist, Checkout, Library, Personal Reviews |
| **Employee** | `employee@gameit.com` | `P@ssword1` | Staff catalog inspection |

---

## 📁 Solution Architecture

The solution follows strict Clean Architecture dependency boundaries:

```text
GameIt/
├── Directory.Build.props
├── GameIt.sln
├── Dockerfile.api
├── Dockerfile.blazor
├── docker-compose.yml
├── .env.example
├── .github/workflows/ci-cd.yml
├── docs/                                  # Comprehensive Enterprise Documentation
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
│   └── adr/                               # Architecture Decision Records
│       ├── 0001-clean-architecture-and-contracts.md
│       ├── 0002-blazor-wasm-core-shared-features.md
│       └── 0003-server-side-pricing-and-security-hardening.md
├── src/
│   ├── Core/
│   │   ├── GameIt.Domain/                 # Pure domain entities (zero NuGet dependencies)
│   │   ├── GameIt.Contracts/              # Strongly-typed shared DTOs & API models
│   │   └── GameIt.Application/            # CQRS commands, queries, handlers, validators
│   ├── Infrastructure/
│   │   ├── GameIt.Persistence/            # EF Core DbContext, entity configurations, repositories
│   │   ├── GameIt.Identity/               # ASP.NET Core Identity DbContext, JWT AuthService
│   │   └── GameIt.Infrastructure/         # Stripe payments, email delivery, Hangfire jobs
│   └── Presentation/
│       ├── GameIt.API/                    # Web API, RFC 7807 error middleware, rate limiting, /health
│       └── GameIt.BlazorUI/               # Blazor WASM (Core -> Shared -> Features vertical slices)
└── tests/
    ├── GameIt.Application.UnitTests/      # 17 Unit & abuse-case test cases (xUnit, Moq, Shouldly)
    └── GameIt.Persistence.IntegrationTests/ # 5 EF Core repository & database integration tests
```

---

## ⚙️ Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Docker SQL Server, or MonsterASP MSSQL)

### Option A: Local Development (.NET CLI)

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/Alber-Ashraf/GameIt.git
   cd GameIt
   ```

2. **Configure Secrets:**
   Copy `.env.example` to `.env` or set user secrets for `GameIt.API`:
   ```bash
   dotnet user-secrets set "JwtSettings:SecretKey" "your-at-least-32-character-secret-key" --project src/Presentation/GameIt.API
   ```

3. **Restore, Build & Test:**
   ```bash
   dotnet restore GameIt.sln
   dotnet build GameIt.sln --configuration Release
   dotnet test GameIt.sln --configuration Release
   ```

4. **Run Web API & Blazor WebAssembly Storefront:**
   ```bash
   # Terminal 1 — Start ASP.NET Core Web API (Swagger at /swagger, health probe at /health)
   dotnet run --project src/Presentation/GameIt.API --launch-profile https

   # Terminal 2 — Start Blazor WebAssembly Frontend
   dotnet run --project src/Presentation/GameIt.BlazorUI --launch-profile https
   ```

### Option B: Docker Compose

Launch the full stack (SQL Server 2022, Web API, and Blazor WebAssembly via Nginx) with one command:

```bash
cp .env.example .env
docker compose up --build -d
```

| Service | Access URL | Description |
|---|---|---|
| **Blazor Storefront** | `http://localhost:5036` | Blazor WASM served via high-performance Nginx |
| **Web API Root** | `http://localhost:5161/` | Service status JSON |
| **Health Probe** | `http://localhost:5161/health` | Container health endpoint (`200 OK`) |
| **Swagger UI** | `http://localhost:5161/swagger` | Interactive OpenAPI documentation |

### Option C: MonsterASP.NET Hosting (Windows/IIS)
`GameIt` is pre-configured for MonsterASP.NET single-MSSQL database hosting:
- Automatic startup migrations via `Database:ApplyMigrationsOnStartup = true`.
- IIS SPA fallback URL rewriting (`wwwroot/web.config`) for client-side Blazor routing.
- See the complete step-by-step guide in [docs/07-deployment.md](docs/07-deployment.md#2-monsteraspnet-deployment-guide-step-by-step).

---

## 🧪 Testing & Verification

The solution includes automated unit and integration tests passing with 0 failures:

```bash
dotnet test GameIt.sln --configuration Release --logger "console;verbosity=normal"
```

- **`GameIt.Application.UnitTests` (17 tests):** Authoritative price calculation, discount mathematics, free game library provisioning, review ownership & IDOR protection, empty query resilience.
- **`GameIt.Persistence.IntegrationTests` (5 tests):** EF Core auditing (`DateCreated`, `DateModified`), similar game queries with eager loading, rating-based ordering.

---

## 📚 Documentation Index

The repository includes enterprise engineering documentation in `docs/`:

| Document | Description |
|---|---|
| [**01 — Requirements**](docs/01-requirements.md) | Business goals, personas, user stories, acceptance criteria, MoSCoW scope |
| [**02 — Architecture**](docs/02-architecture.md) | System overview, dependency boundaries, data flow, Mermaid diagrams |
| [**03 — Database**](docs/03-database.md) | Schema design, ER diagram, check constraints, migration strategy |
| [**04 — API Spec**](docs/04-api.md) | REST endpoints, request/response contracts, RFC 7807 problem details |
| [**05 — UI/UX Design**](docs/05-ui-ux.md) | Design token system, vertical slice layout, 4-state UX lifecycle |
| [**06 — Testing Strategy**](docs/06-testing.md) | Test suite inventory, OWASP Top 10 verification, accessibility |
| [**07 — Deployment**](docs/07-deployment.md) | MonsterASP.NET guide, Docker Compose, CI/CD runbook, rollback steps |
| [**08 — Operations**](docs/08-operations.md) | Structured logging, health probes, backup/restore, incident triage |
| [**09 — Contributing**](docs/09-contributing.md) | Developer workflow, branching strategy, Clean Architecture standards |
| [**10 — Roadmap**](docs/10-roadmap.md) | Completed milestones, backlog, future enhancements |
| [**ADRs (`docs/adr/`)**](docs/adr/) | Architecture Decision Records (0001 Clean Architecture, 0002 Blazor WASM, 0003 Pricing) |

---

## 📸 Screenshots

<p align="center">
  <img width="900" alt="Home Storefront" src="https://github.com/user-attachments/assets/1aa732cd-2688-41f5-9046-e14a95be7b1b" />
</p>

<p align="center">
  <img width="900" alt="Catalog & Featured Games" src="https://github.com/user-attachments/assets/809d4dc4-fd28-435c-b81c-4194ff3154f4" />
</p>

<p align="center">
  <img width="900" alt="Game Details & Specs" src="https://github.com/user-attachments/assets/c9692fd2-f13c-4d98-817c-d3e833a2524d" />
</p>

<p align="center">
  <img width="900" alt="User Reviews" src="https://github.com/user-attachments/assets/8d7b002a-684f-4a1c-bfd1-01eb83e43fb9" />
</p>

<p align="center">
  <img width="900" alt="Wishlist" src="https://github.com/user-attachments/assets/115a098b-a972-468c-a094-3d5dc60290cd" />
</p>

<p align="center">
  <img width="900" alt="Checkout & Stripe" src="https://github.com/user-attachments/assets/8a0cfa16-4841-45e3-8a04-6eafc754537d" />
</p>

<p align="center">
  <img width="900" alt="Digital Library" src="https://github.com/user-attachments/assets/61b26cb1-0b00-4db0-8d62-66287ebba19a" />
</p>

<p align="center">
  <img width="900" alt="Purchase History" src="https://github.com/user-attachments/assets/caba72ca-c1b8-4c41-8767-c8be1df6d056" />
</p>

<p align="center">
  <img width="900" alt="Admin Management" src="https://github.com/user-attachments/assets/44d8253f-cc9b-4100-ade1-621dcb7f613d" />
</p>

---

## 💡 Author

Developed with ❤️ by **Alber Ashraf**

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
