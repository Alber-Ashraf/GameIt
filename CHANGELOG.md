Last updated: 2026-11-03

# Changelog

All notable changes to the GameIt solution are documented in this file.

## [Unreleased] — 2026-11-03

### Added (Downloads & Catalog Support)
- Added optional `AdditionalDownloadLink` support across `Game` entity, EF Core migration (`Add_AdditionalDownloadLink_To_Games`), MediatR commands, shared contracts, admin catalog editor, and user library vault (`LibraryPage.razor`) for supplementary game files, DLCs, and extra patches.

### Changed (Auth & Catalog Units)
- Removed hardcoded demo login credentials and quick-login buttons (`user@gameit.com`, `admin@gameit.com`) from `LoginPage.razor`.
- Converted game file size from raw bytes (`FileSizeInBytes`) to megabytes (`FileSizeInMB`) across domain entities, EF Core migration (`Change_FileSize_To_Megabytes`), database seeder, shared contracts (`GameContracts.cs`, `LibraryContracts.cs`), admin game management form, game details, and library presentation.

## [1.1.0] — 2026-10-30

### Fixed (Stripe Payment Fulfillment & Status Reconciliation)
- Fixed issue where Stripe checkout completion left purchases in `Pending` without adding the game to the user's library due to reliance on unforwarded local Stripe webhooks (`api/webhook`).
- Added dual fulfillment with `/api/Payment/confirm-session` endpoint and automatic checkout redirect handler in `LibraryPage.razor` (`?session_id={CHECKOUT_SESSION_ID}`), guaranteeing immediate fulfillment upon return from Stripe Checkout.
- Added `/api/Payment/purchases/{purchaseId}/verify` endpoint with an interactive "Check Status" button on `/purchases` to let users reconcile any stuck `Pending` orders against Stripe.
- Updated `CreatePurchaseCommandHandler` to record the Stripe Checkout Session ID directly on `Purchase.StripePaymentIntentId` for direct session correlation.
- Added automated unit tests (`ConfirmPaymentCommandHandlerTests`) covering payment session confirmation and pending order verification.

### Added (Storefront Catalog Seeder)
- Added `GameItDatabaseSeeder.cs` in `GameIt.Persistence.DatabaseContext` with 50 popular PC games across 10 categories (RPG, Action, Adventure, Strategy, Shooter, Open World, Indie, Sports & Racing, Horror, Simulation).
- Configured real high-resolution Steam CDN header art, accurate pricing/free-to-play flags, system requirements, file sizes, and empty `DownloadLink` fields.
- Seeded 10 active promotional discounts (20% to 75% off) for storefront deals.
- Integrated seeder into `Program.cs` startup with `Database:SeedDataOnStartup` configuration switch.
- Added integration test suite `GameItDatabaseSeederTests.cs` verifying idempotency and data integrity.

### Fixed (Catalog Serialization & Admin Management Polish)
- Fixed `Unable to load catalog data` error on `/admin/games`: mapped `AverageRating` to `0.0` (never `null`) in `GameProfile.cs` and made `AverageRating` nullable-safe (`double?`) in `GameSummaryDto` and `GameDetailDto` so `System.Text.Json` deserialization never fails when games have 0 reviews.
- Wrapped `ProblemDetailsParser.ToApiResultAsync<T>` deserialization in a `try/catch` block returning structured failure messages rather than throwing unhandled client exceptions.
- Added missing `IsFeatured`, `ReleaseDate`, and null-safe `Price` and `CategoryName` mappings in `GamesListDto.cs` and `GameProfile.cs`.
- Aligned `CreateGameCommand` and `UpdateGameCommand` with `PublisherId` and increased description length validation to 1000 characters to match client input.
- Made discount `Description` nullable in `CreateDiscountCommand` and `UpdateDiscountCommand` to prevent model validation failures on optional campaign notes.
- Resolved category creation `"The Name field is required"` bug by syncing `Name` and `CategoryName` across `CategoryContracts.cs`, `CreateCategoryCommand`, and `UpdateCategoryCommand`.
- Added missing `IsDanger`, `OnConfirm`, and `OnCancel` parameters to `ConfirmDialog.razor` across admin and wishlist pages.
- Embedded custom vector logo component (`GameItLogo.razor`) and SVG brand assets (`logo.svg`, `logo-icon.svg`, `favicon.svg`, `placeholder-game.svg`).
- Configured ASP.NET Core Hosted Blazor WebAssembly in `GameIt.API` with `UseBlazorFrameworkFiles()` and `MapFallbackToFile("index.html")` for single-service deployments.

## [1.0.0] — 2026-10-29

### Added (MonsterASP.NET Deployment Hardening & Codebase Cleanup)
- Added `src/Presentation/GameIt.API/appsettings.Production.json` pre-structured for MonsterASP.NET single-MSSQL hosting (`dbXXXXX.public.databaseasp.net`), CORS allowlisting, and Stripe redirect URLs.
- Updated `src/Presentation/GameIt.API/Program.cs` with configurable startup EF Core auto-migration (`Database:ApplyMigrationsOnStartup`), fault-tolerant startup initialization (preventing IIS `HTTP 500.30` crashes), configurable Swagger UI (`Swagger:Enabled`), and an anonymous `GET /` root status endpoint.
- Added `src/Presentation/GameIt.BlazorUI/wwwroot/appsettings.json`, `wwwroot/appsettings.Production.json`, and `wwwroot/web.config` with IIS URL Rewrite SPA fallback rules, `.wasm`/`.webcil`/`.dat`/`.blat` MIME mappings, and security headers for MonsterASP IIS hosting.
- Updated `docs/07-deployment.md` with a step-by-step MonsterASP.NET deployment guide.
- Fixed null-dereference bugs in `CreateGameCommandHandler` (`category == null`), `CreateDiscountCommandHandler` (`game == null`), and `UserService.GetUser` (`user == null`).
- Replaced obsolete EF Core `builder.HasCheckConstraint(...)` calls (`CS0618`) in `ReviewConfiguration` and `DiscountConfiguration` with EF Core 8 `ToTable(..., t => t.HasCheckConstraint(...))`.
- Fixed nullability warnings across `IGameRepository`, `WebhookController`, `IdentityServicesRegistration`, `StripeService`, `PurchaseResult`, and `WishlistListDto`.

### Added (Phase 7 — Operations, Observability & Maintenance Runbook)
- Enhanced `ExceptionMiddleware` (`src/Presentation/GameIt.API/Middlewares/ExeptionMeddleware.cs`) with structured correlation logging (`{Path}`, `{TraceId}`) for unhandled `500` exceptions and `LogLevel.Warning` audit logs for `401 UnauthorizedAccessException` events.
- Created `docs/08-operations.md` documenting structured logging, `/health` probe monitoring, alerting thresholds, backup/restore RPO/RTO routines, NuGet vulnerability auditing (`dotnet list package --vulnerable --include-transitive`), incident runbooks, and the v2.0 roadmap.
- Updated `README.md` and `docs/10-roadmap.md` with the complete documentation index (`docs/01` through `docs/10` and `docs/adr/`).

### Added (Phase 6 — Deployment, Containerization & CI/CD Runbook)
- Added multi-stage `Dockerfile.api` for `GameIt.API` and `Dockerfile.blazor` + `src/Presentation/GameIt.BlazorUI/nginx.conf` (with gzip compression, SPA fallback routing, immutable `/_framework/` caching, and security headers) for `GameIt.BlazorUI`.
- Added `docker-compose.yml` orchestrating SQL Server 2022 (with healthcheck), `GameIt.API`, and `GameIt.BlazorUI`.
- Added anonymous `/health` probe endpoint in `GameIt.API/Program.cs`.
- Added GitHub Actions CI/CD workflow (`.github/workflows/ci-cd.yml`) running restore, release build, unit & integration tests with coverage, artifact publishing, and staging/production promotion gates.
- Created `docs/07-deployment.md` (environment matrix, Docker Compose instructions, EF Core idempotent migration runbook, production security checklist, and rollback procedure) and `docs/09-contributing.md` (local setup, branching strategy, Clean Architecture rules, and PR checklist).

### Added (Phase 5 — Full-Stack Testing, Security Verification & Web Performance QA)
- Added `DeleteReviewCommandHandlerTests` (4 unit tests) verifying review owner deletion, `Administrator` role override, IDOR/BOLA rejection (`UnauthorizedAccessException`), and unauthenticated rejection.
- Added `CreatePurchaseCommandHandlerTests` (4 unit tests) verifying server-side authoritative price calculation against client price tampering, active discount math, automatic `Library` provisioning for free games, and duplicate ownership prevention.
- Added `GameQueriesAndContractsTests` (8 unit test cases including `@Theory` price math) verifying empty-list resilience across `GetAllGamesList`, `GetFeaturedGames`, `GetSimilarGames`, and `GetGamesByCategory`, plus `GameSummaryDto.FinalPrice` calculations.
- Added `GameRepositoryTests` (3 integration tests) verifying `GetSimilarGamesAsync` null-safety when the base game does not exist, eager loading of `Category`, `Discount`, and `Reviews`, and rating-based ordering.
- Aligned controller route aliases across `ReviewController` (`GET api/review/game/{gameId}`, `POST api/review`, `PUT api/review/{id}`), `CategoryController` (`GET api/category/by-name/{name}`), and `DiscountController` (`GET api/discount/active`) with `ApiEndpoints.cs`.
- Updated `ReviewProfile` to preserve `GameId` and `UserId` during `UpdateReviewCommand` mapping, and removed redundant client `UserId` rule from `DeleteReviewCommandValidator`.
- Created `docs/06-testing.md` documenting test execution, 22/22 passing test inventory, OWASP Top 10 verification matrix, Core Web Vitals (`webperf`) audit, and WCAG 2.1 AA accessibility checks.

### Added (Phase 4 — Vertical-Slice Blazor Rebuild & Feature Parity)
- Rebuilt Blazor WebAssembly application into vertical-slice architecture under `Features/` (`Auth`, `Catalog`, `Categories`, `Discounts`, `Reviews`, `Wishlist`, `Library`, `Purchases`, `Admin`).
- Added Slice 1 (`Features/Auth`): `LoginPage.razor`, `RegisterPage.razor`, and `LogoutPage.razor` with `AuthLayout`, demo credential fillers, validation messages, and JWT authentication.
- Added Slice 2 (`Features/Catalog`, `Categories`, `Discounts`, `Reviews`):
  - `StoreHomePage.razor`: Featured spotlight hero, category filter pills, live query search (`q=`), price/rating sorting, and 4 UX states.
  - `GameDetailsPage.razor`: Cover art hero, specs, price and discount badges, wishlist toggle, buy/claim actions, similar titles grid, and community review CRUD (`POST`/`PUT`/`DELETE`) with role/ownership enforcement.
  - `CategoriesPage.razor`: Genre grid with title counts and genre-filtered game views.
  - `DealsPage.razor`: Promotional campaign cards with countdowns and discounted catalog games.
- Added Slice 3 (`Features/Library`, `Wishlist`):
  - `LibraryPage.razor`: Digital game vault displaying owned titles, acquired dates, file sizes, and installer downloads.
  - `WishlistPage.razor`: Saved games manager with instant checkout routing, item removal, and confirmation dialog for clearing.
- Added Slice 4 (`Features/Purchases`):
  - `CheckoutPage.razor`: Order breakdown, promotional discount savings, instant free-game claim, and Stripe checkout handoff.
  - `PurchasesPage.razor`: Complete transaction receipt history with payment status tags and `tabular-nums` formatting.
- Added Slice 5 (`Features/Admin`):
  - `AdminDashboardPage.razor`: Real-time KPI statistics and quick actions guarded by `[Authorize(Roles = "Administrator")]`.
  - `AdminGamesPage.razor`: Comprehensive game CRUD table with create/edit form and deletion dialog.
  - `AdminCategoriesPage.razor`: Genre taxonomy management with create/edit/delete operations.
  - `AdminDiscountsPage.razor`: Promotional campaign manager with percentage and date window controls.
- Retired legacy unreferenced UI services, ViewModels, and AutoMapper dependency in Blazor UI.

### Added (Phase 3 — Solution Restructure, Backend Hardening & Blazor Foundation)
- Flattened repository structure into `src/Core/`, `src/Infrastructure/`, `src/Presentation/`, and `tests/` with centralized `Directory.Build.props` and `.env.example`.
- Added `src/Core/GameIt.Contracts` project with shared DTOs for Auth, Games, Categories, Discounts, Reviews, Wishlists, Purchases, and Libraries.
- Added `ITransactionScope` abstraction in `GameIt.Application` to eliminate EF Core leakage from `IUnitOfWork`.
- Added direct authenticated user endpoints on `LibraryController` (`GET /api/library`), `WishlistController` (`GET /api/wishlist`), and `PaymentController` (`GET /api/payment/purchases`).
- Added fixed-window rate limiting (`AuthPolicy`) on `AuthController` and explicit CORS origin configuration in `GameIt.API/Program.cs`.
- Added Blazor WebAssembly `Core/` infrastructure: `CustomAuthStateProvider`, `JwtAuthorizationMessageHandler`, `ProblemDetailsParser`, `ApiResult<T>`, `ApiEndpoints`, and `ToastService`.
- Added 8 strongly-typed Feature API Clients (`AuthApiClient`, `GamesApiClient`, `CategoriesApiClient`, `DiscountsApiClient`, `ReviewsApiClient`, `WishlistApiClient`, `LibraryApiClient`, `PurchasesApiClient`).
- Added 3-tier CSS token design system (`tokens.css`, `app.css`), `MainLayout`, `AuthLayout`, `Sidebar`, `TopBar`, and 10 `Shared/UI/` primitives (`PageHeader`, `StatCard`, `PriceBadge`, `StarRating`, `EmptyState`, `ErrorBanner`, `SkeletonCardGrid`, `ConfirmDialog`, `ToastContainer`, `GameCard`).

### Fixed & Security Hardened
- Stripped `Microsoft.AspNetCore.Identity.EntityFrameworkCore` from `GameIt.Domain` and removed `GameIt.Identity` reference from `GameIt.Persistence`.
- Enforced `[Authorize(Roles = "Administrator")]` on all `CategoryController` and `DiscountController` mutation endpoints; fixed `CategoryIt.API.Controllers` namespace typo.
- Restored review ownership and administrator role authorization check in `DeleteReviewCommandHandler`.
- Fixed `AuthService.Register` to `await _userManager.CreateAsync(...)` and assign `"User"` role instead of `"Employee"`.
- Fixed `CreatePurchaseCommandHandler` to compute authoritative prices server-side, prevent duplicate ownership purchases, and automatically grant `Library` entries for free games.
- Redacted stack traces in `ExceptionMiddleware` when running outside Development and added HTTP 401 mapping for `UnauthorizedAccessException`.
- Updated list query handlers (`GetAllGamesList`, `GetFeaturedGames`, `GetGamesByCategory`, `GetSimilarGames`, `GetAllCategoriesList`) to return empty lists (`200 OK`) instead of throwing `NotFoundException`.
