Last updated: 2026-10-29

# 09 — Contributing Guide & Engineering Standards

## 1. Local Development Prerequisites

- **.NET 8 SDK** (`8.0.x`)
- **SQL Server** (LocalDB, SQL Server Express/Developer, or Docker Desktop)
- **Git**

---

## 2. Getting Started

1. Clone the repository and restore NuGet packages:
   ```bash
   dotnet restore GameIt.sln
   ```
2. Copy `.env.example` to `.env` (or configure .NET User Secrets for `GameIt.API`):
   ```bash
   cp .env.example .env
   ```
3. Build and run the test suite to verify your local setup:
   ```bash
   dotnet build GameIt.sln --configuration Release
   dotnet test GameIt.sln --configuration Release
   ```
4. Start the API and Blazor WebAssembly UI in separate terminals:
   ```bash
   dotnet run --project src/Presentation/GameIt.API --launch-profile https
   dotnet run --project src/Presentation/GameIt.BlazorUI --launch-profile https
   ```

---

## 3. Git Branching & Commit Conventions

- **`main`:** Production-ready code. Protected branch requiring passing CI and code review.
- **`develop`:** Integration branch for staging deployments.
- **Feature/Fix Branches:** Branch from `develop` using descriptive prefixes:
  - `feat/<slice>-<short-description>` (e.g., `feat/catalog-publisher-filter`)
  - `fix/<slice>-<short-description>` (e.g., `fix/reviews-ownership-check`)
  - `docs/<topic>` (e.g., `docs/deployment-runbook`)
  - `test/<slice>-<short-description>` (e.g., `test/purchase-price-integrity`)

---

## 4. Clean Architecture & Blazor UI Rules

1. **Strict Layer Dependencies:**
   - `GameIt.Domain` must have **zero** external NuGet package dependencies.
   - `GameIt.Application` must only reference `GameIt.Domain` and `GameIt.Contracts` (never `Microsoft.EntityFrameworkCore`).
   - `GameIt.Persistence` must **not** reference `GameIt.Identity`.
2. **Shared Contracts (`GameIt.Contracts`):**
   - Place shared request/response DTOs in `src/Core/GameIt.Contracts/` so both `GameIt.API` and `GameIt.BlazorUI` compile against identical contracts.
3. **Blazor UI Organization (`Core -> Shared -> Features`):**
   - Never inject raw `HttpClient` directly into `.razor` pages; use the feature-scoped `IApiClient` services returning `ApiResult<T>`.
   - Always use CSS variables from `wwwroot/css/tokens.css` (never hardcode hex colors in components).
   - Every page must handle all 4 UX states: **Loading (`<SkeletonCardGrid />`)**, **Empty (`<EmptyState />`)**, **Error (`<ErrorBanner />`)**, and **Populated**.

---

## 5. Pull Request Checklist

Before opening a Pull Request, verify:

- [ ] `dotnet build GameIt.sln --configuration Release` succeeds with `0 Error(s)`.
- [ ] `dotnet test GameIt.sln --configuration Release` passes 100% of unit and integration tests.
- [ ] New or modified endpoints/handlers include corresponding unit/integration tests, including security abuse cases.
- [ ] All catalog mutation endpoints (`POST`, `PUT`, `DELETE`) enforce `[Authorize(Roles = "Administrator")]`.
- [ ] No secrets, API keys, or connection strings are committed to `appsettings*.json`.
- [ ] Matching documentation in `docs/`, `CHANGELOG.md`, and `docs/10-roadmap.md` is updated in the same PR.
