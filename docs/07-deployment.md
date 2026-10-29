Last updated: 2026-10-29

# 07 — Deployment, CI/CD & Release Runbook

## 1. Environment Matrix

| Environment | Branch Trigger | API URL | Blazor UI URL | Database | Stripe Mode |
|---|---|---|---|---|---|
| **Development** | Feature branches (`feat/*`, `fix/*`) | `https://localhost:7040` (`http://localhost:5161`) | `https://localhost:7294` (`http://localhost:5036`) | LocalDB / SQL Server Docker (`localhost:1433`) | Test (`sk_test_...`) |
| **Staging** | `develop` | `https://api-staging.gameit.example.com` | `https://staging.gameit.example.com` | Azure SQL / Managed SQL (Staging) | Test (`sk_test_...`) |
| **MonsterASP.NET** | `main` | `https://your-api-site.runasp.net` | `https://your-blazor-site.runasp.net` | MonsterASP MSSQL (`dbXXXXX.public.databaseasp.net`) | Test / Live |
| **Production** | `main` | `https://api.gameit.example.com` | `https://store.gameit.example.com` | Azure SQL / Managed SQL (Production HA) | Live (`sk_live_...`) |

---

## 2. MonsterASP.NET Deployment Guide (Step-by-Step)

`GameIt` is pre-configured for **MonsterASP.NET** Windows/IIS hosting with a single MonsterASP SQL Server database. Both `GameItDbContext` (storefront schema + seeded catalog), `GameItIdentityDbContext` (ASP.NET Core Identity + seeded Admin/User accounts), and `Hangfire` share the single `GameItDbConnectionString` database automatically without table collisions.

### Step 1: Create the SQL Server Database in MonsterASP
1. Log in to the **MonsterASP.NET Control Panel** -> **Databases** -> **Create Database**.
2. Copy the generated connection details (`Server=dbXXXXX.public.databaseasp.net;Database=dbXXXXX;User Id=dbXXXXX;Password=...`).
3. Ensure `Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;` is appended to the connection string.

### Step 2: Configure & Publish `GameIt.API`
1. Update `src/Presentation/GameIt.API/appsettings.Production.json` with your MonsterASP values (or set them in MonsterASP's Environment Variables / Configuration editor after deployment):
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
     "Cors": {
       "AllowedOrigins": [
         "https://your-blazor-site.runasp.net"
       ]
     },
     "Stripe": {
       "SecretKey": "sk_test_...",
       "PublishableKey": "pk_test_...",
       "WebhookSecret": "whsec_...",
       "SuccessUrl": "https://your-blazor-site.runasp.net/library",
       "CancelUrl": "https://your-blazor-site.runasp.net/"
     },
     "JwtSettings": {
       "SecretKey": "YOUR_MIN_32_CHARACTER_PRODUCTION_SECRET_KEY",
       "Issuer": "GameIt.Api",
       "Audience": "GameItUser",
       "DurationInMinutes": 60
     }
   }
   ```
2. Publish `GameIt.API` in Release mode:
   ```bash
   dotnet publish src/Presentation/GameIt.API/GameIt.API.csproj -c Release -o ./publish/api
   ```
3. In MonsterASP Control Panel, create a website for the API (e.g., `your-api-site.runasp.net`) and deploy `./publish/api` using **Visual Studio Web Deploy** (import the `.PublishSettings` file from MonsterASP) or upload the contents of `./publish/api` into the site's root directory (`wwwroot`).
4. **Automatic Schema & Seed Initialization:** When `GameIt.API` starts for the first time on MonsterASP with `Database:ApplyMigrationsOnStartup = true`, it automatically runs EF Core migrations for both `GameItDbContext` and `GameItIdentityDbContext`, seeds the catalog and default accounts (`admin@gameit.com` / `P@ssword1` and `user@gameit.com` / `P@ssword1`), and initializes Hangfire tables.
5. Verify the API is live by opening:
   - `https://your-api-site.runasp.net/` (returns service status JSON)
   - `https://your-api-site.runasp.net/health` (returns `200 OK` Healthy status)
   - `https://your-api-site.runasp.net/swagger` (opens Swagger UI)

### Step 3: Configure & Publish `GameIt.BlazorUI`
1. Update `src/Presentation/GameIt.BlazorUI/wwwroot/appsettings.Production.json` (and `wwwroot/appsettings.json` if deploying without setting `ASPNETCORE_ENVIRONMENT`) to point to your MonsterASP API URL:
   ```json
   {
     "ApiBaseUrl": "https://your-api-site.runasp.net/"
   }
   ```
2. Publish `GameIt.BlazorUI` in Release mode:
   ```bash
   dotnet publish src/Presentation/GameIt.BlazorUI/GameIt.BlazorUI.csproj -c Release -o ./publish/blazor
   ```
3. In MonsterASP Control Panel, create a second website for the storefront UI (e.g., `your-blazor-site.runasp.net`) and upload the contents of **`./publish/blazor/wwwroot`** (which includes `index.html`, `_framework/`, `css/`, `appsettings.Production.json`, and the IIS `web.config` for SPA routing and `.wasm`/`.webcil` MIME types) directly into the MonsterASP site's `wwwroot` folder.
4. Open `https://your-blazor-site.runasp.net` and verify login, catalog browsing, and client-side route refreshes (`/games`, `/deals`, `/library`).

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
