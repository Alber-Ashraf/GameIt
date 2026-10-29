Last updated: 2026-10-29

# 08 — Operations, Observability & Maintenance Runbook

## 1. Observability & Structured Logging

### Logging Architecture
- **Correlation & Error Tracking:** `ExceptionMiddleware` (`src/Presentation/GameIt.API/Middlewares/ExeptionMeddleware.cs`) logs unhandled server exceptions at `LogLevel.Error` and unauthorized/IDOR access attempts (`UnauthorizedAccessException`) at `LogLevel.Warning`, enriched with `{Path}` and `{TraceId}` (`HttpContext.TraceIdentifier`).
- **Sensitive Data Redaction:** Passwords, JWT tokens, Stripe API keys, and raw payment card details are never logged. Stack traces are redacted from HTTP responses outside `Development`.
- **Background Job Telemetry:** Hangfire manages background jobs (such as `DiscountExpirationService` expiring elapsed promotional campaigns) and records job execution history, retries, and failures in the `Hangfire.*` SQL Server tables.

---

## 2. Health Checks, Uptime & Alerting Thresholds

### Health Probe
- **Endpoint:** `GET /health`
- **Expected Response (`200 OK`):**
  ```json
  {
    "status": "Healthy",
    "service": "GameIt.API",
    "timestamp": "2026-10-29T14:00:00Z"
  }
  ```

### Recommended Production Alert Rules

| Metric / Signal | Warning Threshold | Critical (Page) Threshold | Action / Playbook |
|---|---|---|---|
| **API Availability (`GET /health`)** | 1 failed probe (30s interval) | 3 consecutive failed probes | Check container status; trigger rollback per `docs/07-deployment.md` |
| **HTTP `5xx` Error Rate** | `> 1%` over 5 minutes | `> 5%` over 5 minutes | Inspect `ExceptionMiddleware` logs by `TraceId`; check SQL Server connectivity |
| **HTTP `429` Rate Limit (`AuthPolicy`)** | `> 50` rejections / 5 min | `> 250` rejections / 5 min | Potential credential-stuffing attack; block offending IP range at WAF/CDN |
| **HTTP `401` Unauthorized Spike** | `> 100` rejections / 5 min | `> 500` rejections / 5 min | Check `JwtSettings` clock skew or potential IDOR enumeration |
| **Stripe Webhook Failures (`POST /api/webhook`)** | Any `400`/`500` webhook response | `> 3` consecutive failures | Verify `Stripe:WebhookSecret` matches Stripe Dashboard signing secret |
| **API P95 Latency** | `> 400ms` | `> 1,200ms` | Inspect SQL Server index usage and connection pool saturation |

---

## 3. Database Backup & Recovery Routine

- **Automated Backups:**
  - Full database backup daily at `02:00 UTC`.
  - Differential backup every 6 hours.
  - Transaction log backup every 15 minutes (enabling Point-In-Time Recovery with `RPO <= 15 minutes`, `RTO <= 30 minutes`).
- **Retention Policy:**
  - Daily backups retained for 30 days.
  - Monthly backups retained for 12 months for financial/purchase audit compliance.
- **Quarterly Restore Drill:**
  - Restore the latest production backup into an isolated staging database once per quarter and run `dotnet test GameIt.sln --configuration Release` to verify schema and data integrity.

---

## 4. Dependency & Security Maintenance Routine

1. **Weekly NuGet Vulnerability Audit:**
   Run the native .NET package vulnerability scanner across all 10 projects:
   ```bash
   dotnet list GameIt.sln package --vulnerable --include-transitive
   ```
   **Current Audit Triage (2026-10-29):**
   - `GameIt.Domain`, `GameIt.Contracts`, and `GameIt.BlazorUI`: **0 vulnerable packages** (we removed `AutoMapper` from `GameIt.BlazorUI` in Phase 4).
   - `AutoMapper 12.0.1` (`GHSA-rvv3-g6hj-g44x` in `GameIt.Application`): Reachability is mitigated because `GameIt` only maps strongly-typed DTOs and domain entities with flat/explicit profiles and never maps untrusted polymorphic object graphs. Upgrading to `AutoMapper 13+` is scheduled for a dedicated dependency upgrade PR because `AutoMapper 13+` drops `AutoMapper.Extensions.Microsoft.DependencyInjection` and requires `AddAutoMapper` signature changes.
   - Transitive `System.Security.Cryptography.Pkcs` (`8.0.1`) & `System.Security.Cryptography.Xml` (`8.0.2`) in `GameIt.Identity`: Transitive dependencies of `Microsoft.AspNetCore.Identity`; GameIt uses HMAC-SHA256 JWT tokens (`JwtSecurityTokenHandler`) and does not parse untrusted PKCS#7 or SignedXml payloads. Pinning explicit patched versions (`8.0.4+`) is tracked in the monthly dependency maintenance branch.
2. **Monthly Patch Cadence:**
   - Check outdated packages:
     ```bash
     dotnet list GameIt.sln package --outdated
     ```
   - Apply patch/minor updates on a dedicated `chore/deps-YYYY-MM` branch, verify all 22 automated tests pass, and deploy to `staging` before merging to `main`.
3. **Secret Rotation Schedule (Every 90 Days):**
   - Rotate `JwtSettings:SecretKey`, `Stripe:SecretKey`, `Stripe:WebhookSecret`, `EmailSettings:ApiKey`, and SQL Server credentials in the secret manager without committing changes to git.

---

## 5. Incident Troubleshooting Guide

### Scenario A: User Paid via Stripe Checkout, but Game Is Not in Library
1. Query the `Purchases` table for the user's `UserId` and `GameId` (`Status = Pending`).
2. Check Stripe Dashboard -> Developers -> Webhooks for delivery status of `checkout.session.completed`.
3. Confirm `Stripe:WebhookSecret` is configured and `POST /api/webhook` is reachable over HTTPS.
4. Re-send the `checkout.session.completed` event from Stripe Dashboard; `WebhookController` will atomically update `Purchase.Status = Succeeded` and insert the `Libraries` record via `ITransactionScope`.

### Scenario B: Expired Discount Still Appears Active
1. Verify Hangfire recurring job `DiscountExpirationService` is scheduled and running.
2. Note that `CreatePurchaseCommandHandler` also enforces `game.Discount.StartDate <= DateTime.UtcNow && game.Discount.EndDate >= DateTime.UtcNow` at checkout time, so expired discounts are never applied to purchases even before the background job runs.

---

## 6. Post-MVP (v2.0) Roadmap

- **HttpOnly Cookie BFF Authentication:** Introduce a Backend-for-Frontend (BFF) cookie proxy so JWTs are stored in `HttpOnly; Secure; SameSite=Strict` cookies rather than browser `localStorage`.
- **Server-Side Catalog Pagination & Full-Text Search:** Add paged `PagedResult<GameSummaryDto>` endpoints (`?page=1&pageSize=24&q=...`) backed by SQL Server full-text indexing for catalogs exceeding 10,000 titles.
- **Publisher Self-Service Portal:** Dedicated `Publisher` role allowing verified studios to upload build artifacts, manage patch notes, and view title revenue analytics.
- **OpenTelemetry Distributed Tracing:** Export traces and metrics from `GameIt.API`, EF Core, and Stripe HTTP calls to Prometheus / Grafana / Azure Monitor.
