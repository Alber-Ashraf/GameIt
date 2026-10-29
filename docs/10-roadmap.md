Last updated: 2026-10-29

# 10 — Prioritized Execution Roadmap & Backlog

## Current Status
- **Phase 1 (Full-Stack Discovery, Gap Matrix & Architectural Audit):** Completed (2026-10-29)
- **Phase 2 (Target Architecture, Solution Restructure & Enterprise UI/UX Plan):** Completed (2026-10-29)
- **Phase 3 (Safe Solution Restructuring, Backend Completion & Blazor Foundation):** Completed (2026-10-29)
- **Phase 4 (Vertical-Slice Rebuild & Missing Pages Implementation):** Completed (2026-10-29)
- **Phase 5 (Full-Stack Testing, Performance & Visual QA):** Completed (2026-10-29)
- **Phase 6 (Deployment, Containerization & CI/CD Runbook):** Completed (2026-10-29)
- **Phase 7 (Operations, Observability & Maintenance Runbook):** Completed (2026-10-29)

---

## Execution Backlog (Milestones 1–4)

### Milestone 1: Safe Solution & Folder Restructuring + Contract Extraction (Phase 3.1) — DONE
- [x] **Epic 1.1 — Repository Flattening & Clean Hygiene**
  - Flattened `GameIt/GameIt.Api/` into `src/Core/`, `src/Infrastructure/`, `src/Presentation/`, and `tests/`.
  - Removed `.vs/` and `GameIt.slnLaunch.user` clutter and added root `Directory.Build.props`.
- [x] **Epic 1.2 — Physical `src/` & `tests/` Migration**
  - Created `src/Core/GameIt.Contracts/` containing shared DTOs, Requests, Responses, and Enums.
  - Recreated `GameIt.sln` with Solution Folders (`Core`, `Infrastructure`, `Presentation`, `Tests`) and verified `dotnet build` and `dotnet test` pass with 0 errors.

### Milestone 2: Backend Continuation, Security Hardening & Contract Alignment (Phase 3.2) — DONE
- [x] **Epic 2.1 — Critical Security & Access Control Hardening**
  - Added `[Authorize(Roles = "Administrator")]` to mutation endpoints in `CategoryController` and `DiscountController` and fixed `CategoryController` namespace.
  - Added `[AllowAnonymous]` to public read endpoints across `GameController`, `CategoryController`, `DiscountController`, and `ReviewController`.
  - Restored and enforced ownership + Admin role check in `DeleteReviewCommandHandler`.
  - Fixed `AuthService.Register` to `await _userManager.CreateAsync(...)` and assign `"User"` role instead of `"Employee"`.
  - Sanitized `ExceptionMiddleware` so stack traces are only included in Development, and added `UnauthorizedAccessException` (`401`) mapping.
  - Replaced hardcoded credentials in `appsettings.json` with safe development defaults/placeholders and created `.env.example`.
- [x] **Epic 2.2 — Payment Integrity, Query Fixes & Clean Dependencies**
  - Computed authoritative purchase price server-side in `CreatePurchaseCommandHandler` (accounting for `IsFree` and active `Discount`), prevented duplicate ownership purchases, and automatically added free games (`AmountPaid == 0`) to `Libraries`.
  - Fixed list QueryHandlers (`GetAllGamesList`, `GetFeaturedGames`, `GetGamesByCategory`, `GetSimilarGames`, `GetAllCategoriesList`) to return `[]` (`200 OK`) instead of throwing `NotFoundException` when empty.
  - Fixed `GameRepository` null-safety and `.Include(g => g.Discount)` eager loading.
  - Removed `Microsoft.AspNetCore.Identity.EntityFrameworkCore` from `GameIt.Domain`, decoupled `GameIt.Persistence` from `GameIt.Identity`, and replaced EF Core `IDbContextTransaction` in `IUnitOfWork` with `ITransactionScope`.

### Milestone 3: Blazor Core Infrastructure, Auth Pipeline & Enterprise Design System (Phase 3.3) — DONE
- [x] **Epic 3.1 — Core HTTP & Authentication Pipeline (`Core/`)**
  - Referenced `GameIt.Contracts` from `GameIt.BlazorUI`.
  - Implemented `CustomAuthStateProvider` (UTC-safe `ValidTo < DateTime.UtcNow` + role claim parsing) and `JwtAuthorizationMessageHandler`.
  - Implemented `ProblemDetailsParser`, `ApiResult<T>`, `ApiEndpoints`, and all 8 Feature API Clients (`Auth`, `Games`, `Categories`, `Discounts`, `Reviews`, `Wishlist`, `Library`, `Purchases`).
- [x] **Epic 3.2 — Enterprise Design System & Shared UI Kit (`wwwroot/css/tokens.css`, `Shared/Layout/`, `Shared/UI/`)**
  - Created `tokens.css` & `app.css` implementing the Deep Zinc/Obsidian (`#09090B`, `#121215`) + Electric Indigo (`#6366F1`) theme with `Inter` and `JetBrains Mono` (`tabular-nums`).
  - Built `MainLayout.razor`, `AuthLayout.razor`, `Sidebar.razor`, `TopBar.razor`, and `ToastService`.
  - Built all 10 `Shared/UI/` primitives: `PageHeader`, `StatCard`, `PriceBadge`, `StarRating`, `EmptyState`, `ErrorBanner`, `SkeletonCardGrid`, `ConfirmDialog`, `ToastContainer`, `GameCard`.

### Milestone 4: Vertical-Slice Blazor Rebuild & Missing Pages Implementation (Phase 4) — DONE
- [x] **Slice 1 — `Features/Auth`:** `LoginPage.razor` (`/login`), `RegisterPage.razor` (`/register`), `LogoutPage.razor` (`/logout`) with `AuthLayout`, quick demo credentials, input validation, and auth state notifications.
- [x] **Slice 2 — `Features/Catalog`, `Categories`, `Discounts`, & `Reviews`:**
  - `StoreHomePage.razor` (`/`, `/games`): Featured hero spotlight banner, genre pills, query search, price/rating sorting, and 4 UX states.
  - `GameDetailsPage.razor` (`/games/{Id:guid}`, `/games/details/{Id:guid}`): Full game specs, price & discount badges, buy/claim CTA, wishlist toggle, similar games carousel, and complete community reviews CRUD with ownership/admin checks and interactive `<StarRating />`.
  - `CategoriesPage.razor` (`/categories`, `/categories/{CategoryId:guid}`): Genre catalog with game counts and filtered game grids.
  - `DealsPage.razor` (`/deals`): Active promotional campaign cards with countdowns and discounted titles grid.
- [x] **Slice 3 — `Features/Library` & `Wishlist`:**
  - `LibraryPage.razor` (`/library`): Authenticated user's owned games, acquisition timestamps, file sizes, and digital installer triggers.
  - `WishlistPage.razor` (`/wishlist`): Authenticated user's saved titles, instant checkout handoff, remove item, and confirmation dialog for clearing wishlist.
- [x] **Slice 4 — `Features/Purchases`:**
  - `CheckoutPage.razor` (`/checkout/{GameId:guid}`): Invoice breakdown, discount calculations, instant free game claim, and Stripe checkout redirect.
  - `PurchasesPage.razor` (`/purchases`): Order history table with status badges, dates, and amounts in `tabular-nums`.
- [x] **Slice 5 — `Features/Admin`:**
  - `AdminDashboardPage.razor` (`/admin`): Real-time KPI stat cards and management shortcuts guarded by `[Authorize(Roles = "Administrator")]`.
  - `AdminGamesPage.razor` (`/admin/games`): Full catalog management table, create/edit modal form, and deletion dialog.
  - `AdminCategoriesPage.razor` (`/admin/categories`): Full category CRUD table and editor form.
  - `AdminDiscountsPage.razor` (`/admin/discounts`): Promotional campaign CRUD with date window validation.
