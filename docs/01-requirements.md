Last updated: 2026-10-29

# 01 — Requirements, Full-Stack Discovery & Architectural Audit

## 1. System Overview & Auto-Deduced Hosting Map

### 1.1 Business Domain & Core Workflows
**GameIt** is a digital PC game storefront and distribution platform (similar to Steam / Epic Games Store) supporting:
1. **Storefront & Catalog Discovery:** Browsing games, featured games carousel, category filtering, active discount promotions, and similar game recommendations based on category and average review rating.
2. **Digital Commerce & Stripe Checkout:** Purchasing paid games via Stripe Checkout Sessions (`checkout.session.completed`, `checkout.session.async_payment_succeeded`, `checkout.session.async_payment_failed` webhooks) or claiming free games (`IsFree == true` / `$0.00`), recording `Purchase` history, and provisioning games into the user's `Library`.
3. **Personal Game Library & Wishlist:** Viewing owned games (`Library`) with download links and system requirements, plus managing a personal `Wishlist` (add, remove, clear).
4. **Community Reviews & Ratings:** Submitting, editing, and deleting 1–5 star ratings and text reviews on games (`Review`).
5. **Administration & Publisher Management:** Creating, updating, and deleting `Game` catalog entries, `Category` taxonomies, and time-bound `Discount` promotions, backed by Hangfire recurring hourly jobs (`UpdateDiscountStatusesAsync` and `RemoveExpiredDiscountsAsync`).

### 1.2 User Roles & Personas
Seeded in `GameIt.Identity/Configurations/RoleConfiguration.cs` and `UserConfiguration.cs`:
- **`Administrator`** (`admin@gameit.com`): Full platform administration (Catalog CRUD, Category CRUD, Discount CRUD, Review moderation).
- **`Employee`** (`employee@gameit.com`): Staff/Publisher tier (currently assigned by default on registration due to a bug in `AuthService.cs:75`).
- **`User`** (`user@gameit.com`): Standard customer browsing the catalog, managing wishlists, purchasing games, accessing their library, and posting reviews.

### 1.3 Technology Stack & Hosting Model
- **Runtime & Framework:** .NET 8.0 (`net8.0`) across all 9 projects.
- **Backend API (`GameIt.API`):** ASP.NET Core Web API with Controllers, MediatR 11.1.0 (CQRS), FluentValidation 12.0.0, AutoMapper 12.0.1, Swashbuckle/Swagger 6.6.2.
- **Database & ORM (`GameIt.Persistence` & `GameIt.Identity`):** Entity Framework Core 8.0.18 with SQL Server (`db_GameIt`), split across two `DbContext` classes (`GameItDbContext` for domain entities and `GameItIdentityDbContext` for ASP.NET Core Identity).
- **Authentication & Authorization (`GameIt.Identity`):** ASP.NET Core Identity with JWT Bearer authentication (`JwtSettings`, 15-minute access token expiry, `uid` custom claim).
- **Background Jobs & External Integrations (`GameIt.Infrastructure`):**
  - **Payments:** `Stripe.net` 48.3.0 (Checkout Sessions + Webhook signature verification).
  - **Background Processing:** `Hangfire` 1.8.20 with SQL Server storage.
  - **Transactional Email:** `SendGrid` 9.29.3 (`IEmailSender`).
- **Frontend (`GameIt.BlazorUI`):**
  - **Hosting Model:** Standalone **Blazor WebAssembly** (`Microsoft.NET.Sdk.BlazorWebAssembly`, `net8.0`), communicating with `GameIt.API` (`https://localhost:7040`) via an NSwag-generated `ServiceClient` (`IClient`), `Blazored.LocalStorage` 4.5.0 for JWT storage, and custom `ApiAuthenticationStateProvider`.
  - **Current Styling:** Stock Bootstrap 5 (`wwwroot/css/bootstrap/bootstrap.min.css`), Bootstrap Icons 1.11.1 CDN, and ad-hoc component CSS.

---

## 2. Solution & Folder Structure Audit

1. **Redundant Nested Repository Root (`D:\Projects\GameIt\GameIt.Api\`):**
   - The true workspace directory `D:\Projects\GameIt` contains only a single wrapper folder `GameIt.Api\`, inside which `.git`, `.github`, `.gitignore`, `.gitattributes`, `GameIt.sln`, `GameIt.slnLaunch.user`, and all 9 project folders reside.
2. **Empty CI/CD Workflows & Local Clutter:**
   - `GameIt.Api\.github\workflows\` exists as an empty directory with zero workflow YAML files.
   - `GameIt.Api\.vs\` and `GameIt.Api\GameIt.slnLaunch.user` sit in the solution root alongside project folders.
   - Flat project layout: Domain, Application, Infrastructure, Persistence, Identity, API, BlazorUI, and Test projects are all dumped in a single flat directory without `src/` (`Core`, `Infrastructure`, `Presentation`) or `tests/` physical separation or Solution Folders in `GameIt.sln`. No `Directory.Build.props` exists.
3. **Clean Architecture Dependency Violations Across Projects:**
   - `GameIt.Domain\GameIt.Domain.csproj:10` references `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.17) even though `GameIt.Domain` should have zero framework/infrastructure dependencies.
   - `GameIt.Application\GameIt.Application.csproj:15-16` directly references `Microsoft.AspNetCore.Http.Abstractions` and `Stripe.net` (48.3.0), leaking HTTP context and payment vendor SDKs into the core application layer.
   - `GameIt.Persistence\GameIt.Persistence.csproj:20` has a `<ProjectReference>` to `GameIt.Identity`, coupling domain persistence to identity infrastructure.
   - `GameIt.Infrastructure\GameIt.Infrastructure.csproj:23` has a `<ProjectReference>` to `GameIt.Persistence`, and `DiscountUpdate.cs:9` injects `GameItDbContext` directly instead of using an application/repository abstraction.
   - `GameIt.Persistence\Repositories\WishlistRepository.cs:1,13` returns the application layer's `WishlistListDto` directly from a persistence repository method.
   - `GameIt.BlazorUI` has no shared `GameIt.Contracts` project; instead it relies on a massive NSwag-generated `Services/Base/ServiceClient.cs` (~2,000+ lines) that duplicates commands (`CreateGameCommand`, `UpdateGameCommand`) and DTOs in `GameIt.BlazorUI.Services.Base` and maps them again via AutoMapper to redundant ViewModels (`GamesListVM`, `GameDetailsVM`).

---

## 3. Backend-to-Blazor Feature Parity Matrix

| Module | Backend Controller & Endpoint | CQRS Command / Query / Service | Blazor UI Page / Route | Status | Audit Notes |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Auth** | `POST /api/Auth/login` | `IAuthService.Login` | `/login` (`Pages/Auth/Login.razor`) | ⚠️ Partial | Basic unstyled form; always sets error message even after successful login before redirect (`Login.razor.cs:34`); no loading spinner. |
| **Auth** | `POST /api/Auth/register` | `IAuthService.Register` | `/register` (`Pages/Auth/Register.razor`) | ⚠️ Partial | Password field is plain text (`<InputText>` without `type="password"` at `Register.razor:42`); missing `ProfilePictureUrl` field; assigns `"Employee"` role instead of `"User"` on backend. |
| **Auth** | Client-side session termination | `IAuthenticationService.Logout` | `/logout` (`Pages/Auth/Logout.razor`) | ✅ Exists / Needs Redesign | Clears `localStorage` token and redirects to `/`. |
| **Catalog (Games)** | `GET /api/Game` | `GetAllGamesListQuery` | `/games/` (`Pages/Game/Index.razor`) | ⚠️ Partial |Requires `[Authorize]` (`Index.razor:2`) blocking anonymous shoppers! Exposes Admin Create/Edit/Delete buttons to all users (`Index.razor:8,84-91`). Discount math bug (`Index.razor:66`). |
| **Catalog (Games)** | `GET /api/Game/featured` | `GetFeaturedGamesQuery` | `/` (`Pages/Home.razor`) | ⚠️ Partial | Hardcoded categories array (`Home.razor.cs:28-31`); discount math bug (`Home.razor:44`); no loading, error, or empty state handling. |
| **Catalog (Games)** | `GET /api/Game/{id}` | `GetGameDetailsQuery` | `/games/details/{id}` | ❌ Missing | Linked from `Home.razor:56` and `Index.razor.cs:28`, but route/page does not exist (404 Not Found). |
| **Catalog (Games)** | `GET /api/Game/category/{categoryId}` | `GetGamesByCategoryQuery` | `/games/category/{categoryId}` | ❌ Missing | Method exists in `GameService.cs:72`, but no UI page or filter exists. |
| **Catalog (Games)** | `GET /api/Game/similar/{gameId}` | `GetSimilarGamesQuery` | Embedded on `/games/details/{id}` | ❌ Missing | Method exists in `GameService.cs:78`, but never called or rendered in UI. |
| **Admin (Games)** | `POST /api/Game` | `CreateGameCommand` | `/games/create` / `/admin/games` | ❌ Missing | Navigated to from `Index.razor.cs:24`, but page does not exist. `GameService.CreateAsync` never attaches Bearer token. |
| **Admin (Games)** | `PUT /api/Game/{id}` | `UpdateGameCommand` | `/games/edit/{id}` / `/admin/games` | ❌ Missing | Navigated to from `Index.razor.cs:26`, but page does not exist. `MappingConfig.cs` is missing `GameDetailsVM -> UpdateGameCommand` map! |
| **Admin (Games)** | `DELETE /api/Game/{id}` | `DeleteGameCommand` | Button on `/games/` (`Index.razor:88`) | ⚠️ Partial | No confirmation dialog; `GameService.Delete` does not call `AddBearerToken()`, so API always returns `401 Unauthorized`. |
| **Categories** | `GET /api/Category` | `GetAllCategoriesListQuery` | `/` & `/categories` & Admin | ❌ Missing | `ICategoryService` (`Contracts/ICategoryService.cs`) and `CategoryService.cs` are completely empty `{}` stubs. |
| **Categories** | `GET /api/Category/{id}` | `GetCategoryDetailsQuery` | `/categories/{id}` | ❌ Missing | `CategoryService.cs` is an empty stub; no page exists. |
| **Categories** | `GET /api/Category/name/{name}` | `GetCategoryByNameQuery` | Category search/lookup | ❌ Missing | `CategoryService.cs` is an empty stub; no UI integration. |
| **Admin (Categories)** | `POST /api/Category` | `CreateCategoryCommand` | `/admin/categories` | ❌ Missing | Empty service stub; no UI exists. Backend endpoint also lacks `[Authorize(Roles = "Administrator")]`. |
| **Admin (Categories)** | `PUT /api/Category/{id}` | `UpdateCategoryCommand` | `/admin/categories` | ❌ Missing | Empty service stub; no UI exists. Backend endpoint lacks `[Authorize(Roles = "Administrator")]`. |
| **Admin (Categories)** | `DELETE /api/Category/{id}` | `DeleteCategoryCommand` | `/admin/categories` | ❌ Missing | Empty service stub; no UI exists. Backend endpoint lacks `[Authorize(Roles = "Administrator")]`. |
| **Discounts** | `GET /api/Discount` | `GetActiveDiscountsQuery` | `/deals` & Storefront Banners | ❌ Missing | `IDiscountService` and `DiscountService.cs` are empty `{}` stubs; no Deals page exists. |
| **Admin (Discounts)** | `POST /api/Discount` | `CreateDiscountCommand` | `/admin/discounts` | ❌ Missing | Empty service stub; no UI exists. Backend endpoint lacks `[Authorize(Roles = "Administrator")]`. |
| **Admin (Discounts)** | `PUT /api/Discount/{id}` | `UpdateDiscountCommand` | `/admin/discounts` | ❌ Missing | Empty service stub; no UI exists. Backend endpoint lacks `[Authorize(Roles = "Administrator")]`. |
| **Admin (Discounts)** | `DELETE /api/Discount/{id}` | `DeleteDiscountCommand` | `/admin/discounts` | ❌ Missing | Empty service stub; no UI exists. Backend endpoint lacks `[Authorize(Roles = "Administrator")]`. |
| **Reviews** | `GET /api/Review/reviews/{gameId}` | `GetReviewsByGameQuery` | Game Details Reviews Section | ❌ Missing | `IReviewService` and `ReviewService.cs` are empty `{}` stubs; no UI exists. |
| **Reviews** | `POST /api/Review/{gameId}` | `CreateReviewCommand` | Game Details Write Review | ❌ Missing | Empty service stub; no UI exists. |
| **Reviews** | `PUT /api/Review/{id}/{gameId}` | `UpdateReviewCommand` | Game Details Edit Review | ❌ Missing | Empty service stub; no UI exists. |
| **Reviews** | `DELETE /api/Review/{id}` | `DeleteReviewCommand` | Game Details / Admin Moderation | ❌ Missing | Empty service stub; no UI exists. |
| **Wishlist** | `GET /api/Wishlist/users/{userId}` | `GetUserWishlistListQuery` | `/wishlist` | ❌ Missing | `IWishlistService` and `WishlistService.cs` are empty `{}` stubs; no page exists. |
| **Wishlist** | `POST /api/Wishlist/items` | `AddToWishlistCommand` | Storefront / Game Details CTA | ❌ Missing | Empty service stub; no UI button or state exists. |
| **Wishlist** | `DELETE /api/Wishlist/items/{gameId}` | `RemoveFromWishlistCommand` | `/wishlist` & Game Card | ❌ Missing | Empty service stub; no UI exists. |
| **Wishlist** | `DELETE /api/Wishlist` | `ClearWishlistCommand` | `/wishlist` Clear CTA | ❌ Missing | Empty service stub; no UI exists. |
| **Checkout & Purchases** | `POST /api/Payment/purchases` | `CreatePurchaseCommand` | `/checkout` / Buy / Claim Free CTA | ❌ Missing | `IPurchaseService` and `PurchaseService.cs` are empty `{}` stubs; no checkout or free-game claim flow in UI. |
| **Purchases** | `GET /api/Payment/users/{userId}/purchases` | `GetUserPurchaseListQuery` | `/purchases` (Order History) | ❌ Missing | Empty service stub; no Order History page exists. |
| **Webhooks** | `POST /api/webhook` | `WebhookController.StripeWebhook` | `/checkout/success` & `/checkout/cancel` | ❌ Missing | Backend handles Stripe webhook, but Blazor UI has no `/checkout/success` or `/checkout/cancel` landing pages (`appsettings.json:23-24` points to `yourdomain.com`). |
| **Library** | `GET /api/Library/users/{userId}` | `GetUserLibraryListQuery` | `/library` (My Games Library) | ❌ Missing | No `ILibraryService` or `LibraryService` even exists in `GameIt.BlazorUI/Contracts` or `Services`, and no `/library` page exists. |

---

## 4. Backend & Blazor Tear-Down Audit (File & Line Citations)

### 4.1 Backend Critical & High Findings
1. **Committed Plaintext Credentials & API Keys (CRITICAL — Security):**
   - `GameIt.API/appsettings.json:10,20-22,28`: Hardcoded SQL Server `sa` password (`139BeRo2`), Stripe SecretKey (`sk_test_...`), PublishableKey, WebhookSecret (`whsec_...`), and JWT signing secret key committed directly to git history.
2. **Stack Trace Disclosure in Production Error Responses (HIGH — Security):**
   - `GameIt.API/Middlewares/ExeptionMeddleware.cs:56-62`: Default exception handler populates `Detail = ex.StackTrace` and `Title = ex.Message` unconditionally for all `500 InternalServerError` responses, leaking internal file paths and implementation details to clients. (File also misspelled `ExeptionMeddleware.cs` and namespace `GameIt.Application.Exeptions`).
3. **Missing `[Authorize(Roles = "Administrator")]` on Category & Discount Mutations (CRITICAL — Broken Access Control / OWASP A01):**
   - `GameIt.API/Controllers/CategoryController.cs:15,61,75,91`: Controller has no `[Authorize]` attributes at all. Any anonymous user can create, update, or delete categories! (Also note namespace typo `namespace CategoryIt.API.Controllers;` at line 11).
   - `GameIt.API/Controllers/DiscountController.cs:12,32,46,63`: Controller has no `[Authorize]` attributes. Any anonymous user can create 100% discounts on any paid game or delete active discounts!
4. **Commented-Out Ownership Check in `DeleteReviewCommandHandler` (CRITICAL — IDOR / BOLA):**
   - `GameIt.Application/Features/Review/Commands/DeleteReview/DeleteReviewCommandHandler.cs:30-33`: The ownership check `if (existingReview == null || existingReview.UserId != request.UserId)` is commented out! Any authenticated user can delete any other user's review by ID.
5. **Privilege Escalation Bug on User Registration (CRITICAL — Broken Access Control):**
   - `GameIt.Identity/Services/AuthService.cs:71-75`: Uses synchronous blocking `result.Result.Succeeded` (deadlock/thread-pool starvation risk) and assigns every newly registered user to the `"Employee"` role (`await _userManager.AddToRoleAsync(user, "Employee")`) instead of `"User"`.
6. **Client-Side Price Tampering & Missing Free-Game Library Provisioning in `CreatePurchaseCommandHandler` (CRITICAL — Business Logic / Integrity):**
   - `GameIt.Application/Features/Purchase/Commands/CreatePurchase/CreatePurchaseCommandHandler.cs:56,63-96`:
     - Trusts `request.AmountPaid` sent by the client instead of computing the authoritative price on the server from `game.Price`, `game.IsFree`, and any active `game.Discount`! An attacker can send `AmountPaid = 0` for a $59.99 game and hit the `else` branch at line 82, marking the purchase `PaymentStatus.Succeeded` for free!
     - Furthermore, when `AmountPaid == 0` (free game), lines 82–93 mark `Purchase.Status = PaymentStatus.Succeeded` but **never create a `Library` entry** (`_unitOfWork.Libraries.CreateAsync(...)`), so free games never appear in the user's library!
7. **Broken Route Parameter & Missing Idempotency in `WebhookController` & `GameController`:**
   - `GameIt.API/Controllers/GameController.cs:100-112`: `UpdateGame` takes `[FromRoute] Guid id` and `[FromBody] UpdateGameCommand command`, but never checks `id == command.Id` or assigns `command.Id = id`.
   - `GameIt.API/Controllers/LibraryController.cs:19`, `PaymentController.cs:21`, `WishlistController.cs:23`: Endpoints declare `users/{userId}` in the route template, ignore the route `userId` parameter (or don't even bind it in `LibraryController` and `WishlistController`), and read `"uid"` from JWT claims instead — creating a confusing, mismatched REST contract.
   - `GameIt.API/Controllers/ReviewController.cs:13,22`: `[Authorize]` is placed at the controller level without `[AllowAnonymous]` on `GetReviewsByGame`, preventing anonymous store visitors from reading game reviews.
   - `GameIt.API/Controllers/WebhookController.cs:109,140-148`: Does not check if `session.Metadata` contains `"purchase_id"` via `TryGetValue` (throws `KeyNotFoundException`), and does not verify whether the user already owns the game in `Libraries` before inserting a duplicate row.
8. **Empty Collection Queries Throw `404 NotFoundException` Instead of Returning `200 OK` with `[]`:**
   - `GetAllGamesListQueryHandler.cs:24-25`, `GetFeaturedGamesQueryHandler.cs:25-26`, `GetGamesByCategoryQueryHandler.cs:27-28`, `GetSimilarGamesQueryHandler.cs:26-27`, `GetAllCategoriesListQueryHandler.cs:25-26`: When a database table or category has 0 games, the query handlers throw `NotFoundException`, causing the API to return `404 Not Found` and crashing the Blazor frontend on initial load instead of returning an empty list `[]`.
9. **EF Core Query & Repository Flaws:**
   - `GameRepository.cs:53-57`: `GetSimilarGamesAsync` fetches `baseGame` without checking for `null` before dereferencing `baseGame.CategoryId` at line 57 (throws `NullReferenceException` when `gameId` does not exist). Also lacks `.Include(g => g.Discount)` on `GetSimilarGamesAsync`, `GetGamesByCategoryAsync`, and `GetFeaturedGamesAsync`.
   - `WishlistRepository.cs:20-26`: Projects `WishlistListDto` without mapping `Game.Name` or `Game.Description`, so wishlist items have no game title!
   - `UnitOfWork.cs:17-23`: Expression-bodied properties (`public IGameRepository Games => new GameRepository(_context);`) allocate a brand-new repository instance on every property access.
   - `GameItDbContext.cs:44` & `AuthService.cs:115`: Uses local `DateTime.Now` instead of `DateTime.UtcNow`.
   - `ApplicationServiceRegistration.cs:9-14`: Does not register FluentValidation validators in DI (`services.AddValidatorsFromAssembly(...)`), forcing every command handler to manually `new` its validator.

### 4.2 Blazor Frontend (`GameIt.BlazorUI`) Tear-Down Findings
1. **85% of Backend Capabilities Are Unimplemented in Blazor:**
   - Only 4 pages exist (`Home.razor`, `Pages/Game/Index.razor`, `Pages/Auth/Login.razor`, `Pages/Auth/Register.razor`, plus `Logout.razor`).
   - 5 of 7 services (`CategoryService`, `DiscountService`, `PurchaseService`, `ReviewService`, `WishlistService`) are empty class bodies `{}` with empty interfaces `{}`. `LibraryService` does not exist at all.
2. **Broken JWT Bearer Token Attachment (`BaseHttpService.cs` & `GameService.cs`):**
   - `BaseHttpService.cs:56-61` defines `protected async Task AddBearerToken()`, **which is never called anywhere in the entire `GameIt.BlazorUI` project!** Consequently, every `[Authorize]` API call (`CreateAsync`, `Update`, `Delete`) fails with `401 Unauthorized`.
3. **Broken Discount Calculation Formula in Razor Views:**
   - `Pages/Home.razor:44` (`game.Game.Price * (1 - (decimal)game.Game.Discount)`) and `Pages/Game/Index.razor:66` (`game.Price * (1 - game.Discount)`): `Discount` is an integer percentage (e.g., `20` for 20%), so `1 - 20 = -19`, rendering a $50 game with a 20% discount as **-$950.00**!
4. **Missing AutoMapper Mapping & Duplicate Layering:**
   - `GameService.cs:88` calls `_mapper.Map<UpdateGameCommand>(game)` (where `game` is `GameDetailsVM`), which is not registered in `MappingConfig.cs:10-20` (throws `AutoMapperMappingException` at runtime).
   - `MappingConfig.cs:18` maps `GameDetailsVM` to `GamesListDto` instead of `GameDetailsDto`!
5. **Authentication & State Bugs:**
   - `Providers/ApiAuthenticationStateProvider.cs:29`: Compares `tokenContent.ValidTo < DateTime.Now` (`ValidTo` is always UTC in `JwtSecurityToken`, while `DateTime.Now` is local browser time, causing immediate token invalidation or expired token usage in non-UTC timezones).
   - `Pages/Auth/Login.razor.cs:30-35` & `Register.razor.cs:29-34`: Missing `else` branch after `NavigationManager.NavigateTo("/")`, unconditionally setting the error `Message` on successful login/registration.
6. **UI/UX & Accessibility Deficiencies:**
   - Uses default Bootstrap template sidebar (`MainLayout.razor`, `NavMenu.razor`) with broken icon classes (mixes OpenIconic `oi oi-*`, Bootstrap Icons `bi bi-*`, and FontAwesome `fas fa-*` when only Bootstrap Icons CDN is loaded in `index.html:13`).
   - Zero `<SkeletonLoader />`, zero `<EmptyState />`, zero `<ErrorBoundary>`, zero toast notifications, zero confirmation modals (deleting a game happens on a single accidental click), and hardcoded dummy category strings on the Home page.
