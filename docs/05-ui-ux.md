Last updated: 2026-10-29

# 05 — Enterprise UI/UX Design System & Blazor Component Contract

## 1. Aesthetic Direction
**High-Craft Gaming Storefront & Enterprise SaaS Hybrid** (Steam / Epic Storefront meets Linear / Stripe Dark Mode):
- **Primary Canvas (`--bg-canvas`):** `#09090B` (Zinc 950)
- **Elevated Surface (`--bg-surface`):** `#121215` (Obsidian Panel)
- **Interactive / Card Hover Surface (`--bg-elevated`):** `#18181B` (Zinc 900)
- **Glass Overlay (`--bg-glass`):** `rgba(18, 18, 21, 0.82)` with `backdrop-filter: blur(12px)`
- **Crisp Hairline Border (`--border-subtle`):** `#27272A` (`1px solid`)
- **Focus / Active Border (`--border-focus`):** `#6366F1` (Electric Indigo)
- **Primary Brand Accent (`--accent-primary`):** `#6366F1` (Hover: `#4F46E5`, Glow: `rgba(99, 102, 241, 0.18)`)
- **Semantic Status Colors:**
  - **Success / Free / Owned (`--color-success`):** `#10B981` (Emerald 500)
  - **Sale / Discount Badge (`--color-warning`):** `#F59E0B` (Amber 500)
  - **Danger / Destructive (`--color-danger`):** `#EF4444` (Red 500)
  - **Info / Pending (`--color-info`):** `#3B82F6` (Blue 500)

## 2. Typography & Scannable Hierarchy
- **UI Sans (`--font-sans`):** `'Inter', -apple-system, BlinkMacSystemFont, sans-serif`
- **Monospace (`--font-mono`):** `'JetBrains Mono', monospace` with `font-variant-numeric: tabular-nums` applied to all **prices, discount percentages, file sizes, dates, order IDs, and KPI counters**.

## 3. Three-Layer Token System (`wwwroot/css/tokens.css`)
1. **Primitive Tokens:** Raw color palette (`--zinc-950`, `--indigo-500`, `--emerald-500`), spacing scale (`4px` base: `--space-1` to `--space-12`), radii (`--radius-sm: 6px`, `--radius-md: 10px`, `--radius-lg: 14px`).
2. **Semantic Tokens:** `--bg-canvas`, `--bg-surface`, `--text-primary` (`#FAFAFA`), `--text-secondary` (`#A1A1AA`), `--text-muted` (`#71717A`), `--border-subtle`.
3. **Component Tokens:** `--card-bg`, `--card-border`, `--btn-primary-bg`, `--input-bg`, `--badge-sale-bg`.

## 4. Mandatory 4 UX States for Every Blazor View
Every page in `GameIt.BlazorUI` must explicitly implement all four states:
1. **Loading State:** `<SkeletonLoader Variant="CardGrid | Table | Detail | KpiRow" Count="..." />` matching the exact geometry of the populated view (zero plain `"Loading..."` spinners).
2. **Empty State:** `<EmptyState Icon="..." Title="..." Description="..." ActionText="..." OnAction="..." />` with a clear call-to-action.
3. **Error State:** Global `<ErrorBoundary>` in `MainLayout.razor` plus inline error alert card with a **"Retry"** button that re-triggers the page's data load method.
4. **Populated State:** High-density cards/tables with search, category filter pills, sort controls, role-gated admin actions, and micro-interaction feedback.

## 5. Complete Page & Route Inventory (`GameIt.BlazorUI`)

| Route | Component | Layout | Access | Primary Purpose |
| :--- | :--- | :--- | :--- | :--- |
| `/` | `StoreHome.razor` | `MainLayout` | Public | Featured hero showcase, live Category pills (from API), Active Deals rail, Storefront catalog preview |
| `/games` | `CatalogPage.razor` | `MainLayout` | Public | Full searchable, sortable, category-filterable Game Catalog with Free/Sale filters & Wishlist quick-toggle |
| `/games/details/{Id:guid}` | `GameDetailsPage.razor` | `MainLayout` | Public | Full game dossier, dynamic discounted pricing, Buy Now / Claim Free / Add to Wishlist CTAs, System Requirements, Similar Games rail, Community Reviews CRUD |
| `/categories` | `CategoriesPage.razor` | `MainLayout` | Public | Browse all categories with game counts and instant lookup by category name |
| `/categories/{Id:guid}` | `CategoryDetailsPage.razor` | `MainLayout` | Public | Category header + all games in that category |
| `/deals` | `DealsPage.razor` | `MainLayout` | Public | Active time-bound discounts (`GET /api/Discount`) with countdown expiration badges and savings math |
| `/wishlist` | `WishlistPage.razor` | `MainLayout` | `[Authorize]` | User's saved wishlist items, Move-to-Checkout CTA, remove item, and Clear Wishlist confirmation |
| `/library` | `LibraryPage.razor` | `MainLayout` | `[Authorize]` | Owned games library (`GET /api/Library`), direct download trigger, file size & system specs modal |
| `/checkout/{GameId:guid}` | `CheckoutPage.razor` | `MainLayout` | `[Authorize]` | Order summary, server-verified discount calculation, instant Free-Game claim or Stripe Checkout redirect |
| `/checkout/success` | `CheckoutSuccessPage.razor` | `MainLayout` | `[Authorize]` | Post-checkout confirmation with direct link to `/library` and `/purchases` |
| `/checkout/cancel` | `CheckoutCancelPage.razor` | `MainLayout` | `[Authorize]` | Graceful cancelled-payment state with retry checkout CTA |
| `/purchases` | `PurchasesPage.razor` | `MainLayout` | `[Authorize]` | Complete order history table (`GET /api/Payment/users/{userId}/purchases`) with payment status badges |
| `/admin` | `AdminDashboardPage.razor` | `MainLayout` | `[Authorize(Roles = "Administrator")]` | Executive KPI cards (Total Games, Categories, Active Discounts, Average Store Rating) & quick management |
| `/admin/games` | `AdminGamesPage.razor` | `MainLayout` | `[Authorize(Roles = "Administrator")]` | Enterprise `DataGrid` for Games with `SlideOverDrawer` Create/Edit form and `ConfirmDialog` Delete |
| `/admin/categories` | `AdminCategoriesPage.razor` | `MainLayout` | `[Authorize(Roles = "Administrator")]` | Enterprise `DataGrid` for Categories with Modal Create/Edit/Delete |
| `/admin/discounts` | `AdminDiscountsPage.razor` | `MainLayout` | `[Authorize(Roles = "Administrator")]` | Enterprise `DataGrid` for Discounts with percentage & date-range validation |
| `/login` | `Login.razor` | `AuthLayout` | Public | High-craft dark authentication card with validation, demo-credential quick-fill buttons, and submit spinner |
| `/register` | `Register.razor` | `AuthLayout` | Public | Registration form with masked password input, validation messages, and auto-redirect |
| `/logout` | `Logout.razor` | `AuthLayout` | Public | Clears JWT session and notifies `CustomAuthStateProvider` |
