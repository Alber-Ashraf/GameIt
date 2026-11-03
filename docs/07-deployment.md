Last updated: 2026-11-03

# 07 — Deployment, CI/CD & Release Runbook

## 1. Environment Matrix

| Environment | Branch Trigger | API URL | Blazor UI URL | Database | Stripe Mode |
|---|---|---|---|---|---|
| **Development** | Feature branches (`feat/*`, `fix/*`) | `https://localhost:7040` (`http://localhost:5161`) | `https://localhost:7040` (Hosted) or `https://localhost:7294` (Standalone) | LocalDB / SQL Server Docker (`localhost:1433`) | Test (`sk_test_...`) |
| **Staging** | `develop` | `https://api-staging.gameit.example.com` | `https://staging.gameit.example.com` | Azure SQL / Managed SQL (Staging) | Test (`sk_test_...`) |
| **MonsterASP.NET** | `master` | `https://your-site.runasp.net` | `https://your-site.runasp.net` (Hosted in `GameIt.API`) | MonsterASP MSSQL (`dbXXXXX.public.databaseasp.net`) | Test / Live |
| **Production** | `master` | `https://api.gameit.example.com` | `https://store.gameit.example.com` | Azure SQL / Managed SQL (Production HA) | Live (`sk_live_...`) |

---

## 2. MonsterASP.NET Deployment Guide (Single-Site Hosted Blazor WASM)

`GameIt.API` is configured as an **ASP.NET Core Hosted Blazor WebAssembly** application (`Microsoft.AspNetCore.Components.WebAssembly.Server`). Publishing and deploying **`GameIt.API` alone** automatically bundles and serves the entire `GameIt.BlazorUI` client interface at the root URL (`/`) alongside the REST API (`/api/*`), Swagger (`/swagger`), and health probe (`/health`) on a single MonsterASP.NET website.

### Step 1: Create the SQL Server Database in MonsterASP
1. Log in to the **MonsterASP.NET Control Panel** -> **Databases** -> **Create Database**.
2. Copy the generated connection details (`Server=dbXXXXX.public.databaseasp.net;Database=dbXXXXX;User Id=dbXXXXX;Password=...`).
3. Ensure `Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;` is appended to the connection string.

### Step 2: Configure & Publish `GameIt.API` (Includes `GameIt.BlazorUI`)
1. Update `src/Presentation/GameIt.API/appsettings.Production.json` (or `appsettings.json`) with your MonsterASP values:
   ```json
   {
     "ConnectionStrings": {
       "GameItDbConnectionString": "Server=dbXXXXX.public.databaseasp.net;Database=dbXXXXX;User Id=dbXXXXX;Password=YOUR_MONSTERASP_DB_PASSWORD;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
     },
     "Database": {
       "ApplyMigrationsOnStartup": true
     },
     "Swagger": {
       "Enabled": true
     },
     "Stripe": {
       "SecretKey": "sk_test_...",
       "PublishableKey": "pk_test_...",
       "WebhookSecret": "whsec_...",
       "SuccessUrl": "https://your-site.runasp.net/library",
       "CancelUrl": "https://your-site.runasp.net/"
     },
     "JwtSettings": {
       "SecretKey": "YOUR_MIN_32_CHARACTER_PRODUCTION_SECRET_KEY",
       "Issuer": "GameIt.Api",
       "Audience": "GameItUser",
       "DurationInMinutes": 60
     }
   }
   ```
2. Publish `GameIt.API` in Release mode (this automatically compiles and bundles `GameIt.BlazorUI` into `wwwroot`):
   ```bash
   dotnet publish src/Presentation/GameIt.API/GameIt.API.csproj -c Release -o ./publish
   ```
3. Deploy `./publish` to your single MonsterASP.NET website using **Visual Studio Web Deploy** (import the `.PublishSettings` file from MonsterASP and publish `GameIt.API`) or upload the contents of `./publish` to your MonsterASP site root.
4. **Automatic Schema & Seed Initialization:** When `GameIt.API` starts for the first time on MonsterASP with `Database:ApplyMigrationsOnStartup = true`, it automatically runs EF Core migrations for both `GameItDbContext` and `GameItIdentityDbContext`, seeds the catalog and default accounts (`admin@gameit.com` / `P@ssword1` and `user@gameit.com` / `P@ssword1`), and initializes Hangfire tables.
5. **Verify Live Endpoints:**
   - `https://your-site.runasp.net/` — Loads the **GameIt Blazor WebAssembly Client UI**
   - `https://your-site.runasp.net/swagger` — Opens Swagger API documentation
   - `https://your-site.runasp.net/health` — Returns `200 OK` health status JSON

---

## 3. Containerized Deployment (`Docker` & `docker-compose`)

The solution includes multi-stage Dockerfiles for both the ASP.NET Core Web API (`Dockerfile.api`) and the standalone Blazor WebAssembly client served via Nginx (`Dockerfile.blazor`), orchestrated by `docker-compose.yml`.

### Quick Start via Docker Compose

1. Copy `.env.example` to `.env` and populate secrets:
   ```bash
   cp .env.example .env
   ```
2. Build and launch SQL Server 2022, `GameIt.API`, and `GameIt.BlazorUI`:
   ```bash
   docker compose up --build -d
   ```
3. Verify container health:
   ```bash
   curl http://localhost:5161/health
   # Returns: {"status":"Healthy","service":"GameIt.API","timestamp":"..."}
   ```
4. Access the storefront at `http://localhost:5036` and the API at `http://localhost:5161`.

---

## 3. CI/CD Pipeline (`.github/workflows/ci-cd.yml`)

Every push and pull request to `develop` or `main` runs the automated GitHub Actions pipeline:

1. **Restore & Cache:** Restores NuGet packages across all 10 projects in `GameIt.sln`.
2. **Release Build:** Compiles the entire solution with `--configuration Release --no-restore`.
3. **Automated Test Gate:** Runs all unit (`GameIt.Application.UnitTests`) and integration (`GameIt.Persistence.IntegrationTests`) tests with code coverage collection. Any failing test blocks artifact publishing and deployment.
4. **Artifact Publishing:** Publishes immutable release bundles for `GameIt.API` and `GameIt.BlazorUI`.
5. **Environment Promotion:**
   - Pushes to `develop` deploy automatically to the `staging` environment.
   - Pushes to `main` deploy to the `production` environment after passing environment protection rules.

---

## 4. Database Migration Strategy

Never run `Database.Migrate()` automatically on startup in multi-instance production environments without an explicit migration gate.

### Generating Idempotent SQL Migration Scripts
```bash
dotnet ef migrations script \
  --project src/Infrastructure/GameIt.Persistence \
  --startup-project src/Presentation/GameIt.API \
  --idempotent \
  --output ./artifacts/migrations/gameit-persistence.sql

dotnet ef migrations script \
  --project src/Infrastructure/GameIt.Identity \
  --startup-project src/Presentation/GameIt.API \
  --idempotent \
  --output ./artifacts/migrations/gameit-identity.sql
```

### Applying Migrations Safely
1. Take a point-in-time database snapshot/backup before applying schema changes.
2. Execute the idempotent SQL scripts against the target database during the deployment pre-flight step.
3. Verify schema version in `__EFMigrationsHistory`.
4. Deploy the new API container revision.

---

## 5. Production Readiness & Security Checklist

- [x] **Secrets Externalized:** Zero credentials in `appsettings.json`; all secrets injected via environment variables (`ConnectionStrings__GameItDbConnectionString`, `JwtSettings__SecretKey`, `Stripe__SecretKey`, `Stripe__WebhookSecret`, `EmailSettings__ApiKey`).
- [x] **HTTPS & Security Headers:** HTTPS redirection active in `GameIt.API`; `X-Frame-Options: SAMEORIGIN`, `X-Content-Type-Options: nosniff`, and `Referrer-Policy: strict-origin-when-cross-origin` configured in `nginx.conf`.
- [x] **Explicit CORS Allowlist:** `Cors:AllowedOrigins` restricted to the exact Blazor frontend origin (`BlazorClientPolicy`).
- [x] **Rate Limiting:** Fixed-window rate limiter (`AuthPolicy`: 10 req/min per IP) active on `/api/auth/login` and `/api/auth/register`.
- [x] **Error Redaction:** `ExceptionMiddleware` suppresses internal stack traces when `ASPNETCORE_ENVIRONMENT != Development`.
- [x] **Health Probe:** `/health` endpoint exposed for container orchestrator liveness/readiness checks.

---

## 6. Rollback Runbook

If a post-deployment health check (`/health`), Stripe webhook verification, or smoke test fails in Staging or Production:

1. **Application Rollback (Immediate — < 2 minutes):**
   - Re-deploy the previous known-good container image tag or GitHub Actions release artifact (`gameit-release-artifacts` from the prior commit SHA).
   - Verify `GET /health` returns `200 OK`.
2. **Database Rollback (If schema was modified):**
   - Prefer backward-compatible additive migrations (expand-contract pattern) so application rollbacks do not require database downgrades.
   - If a destructive migration must be reverted, restore from the pre-deployment point-in-time database backup or generate a targeted down-migration script:
     ```bash
     dotnet ef migrations script <CurrentMigration> <PreviousKnownGoodMigration> \
       --project src/Infrastructure/GameIt.Persistence \
       --startup-project src/Presentation/GameIt.API \
       --output rollback.sql
     ```
3. **Post-Rollback Verification:**
   - Confirm storefront catalog (`GET /api/game`), login (`POST /api/auth/login`), and Stripe webhook processing are healthy.
