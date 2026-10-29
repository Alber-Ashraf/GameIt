Last updated: 2026-10-29

# CLAUDE.md — GameIt Project Instructions

## Overview
GameIt is a .NET 8 Clean Architecture PC digital game storefront with an ASP.NET Core Web API backend (`src/Presentation/GameIt.API`) and a standalone Blazor WebAssembly frontend (`src/Presentation/GameIt.BlazorUI`) sharing strongly-typed contracts via `src/Core/GameIt.Contracts`.

## Tech Stack
- **Backend:** .NET 8, ASP.NET Core Web API, MediatR 12, FluentValidation, AutoMapper, EF Core 8 (SQL Server), ASP.NET Core Identity + JWT Bearer Auth, Hangfire, Stripe.net, SendGrid.
- **Frontend:** Blazor WebAssembly (.NET 8), `Blazored.LocalStorage`, `CustomAuthStateProvider`, `JwtAuthorizationMessageHandler`, 3-tier CSS Token Design System (`wwwroot/css/tokens.css` + `app.css`).
- **Testing:** xUnit, Moq, Shouldly, EF Core InMemory.

## Exact Commands
- **Restore:** `dotnet restore GameIt.sln`
- **Build:** `dotnet build GameIt.sln`
- **Test:** `dotnet test GameIt.sln`
- **Run API:** `dotnet run --project src/Presentation/GameIt.API --launch-profile https` (listens on `https://localhost:7040` and `http://localhost:5161`)
- **Run Blazor UI:** `dotnet run --project src/Presentation/GameIt.BlazorUI --launch-profile https` (listens on `https://localhost:7294` and `http://localhost:5036`)

## Architectural Rules & Conventions
1. **Strict Dependency Rule:**
   - `GameIt.Domain` has **zero** external NuGet dependencies.
   - `GameIt.Persistence` does **not** reference `GameIt.Identity`.
   - `GameIt.Application` does **not** reference EF Core (`ITransactionScope` abstracts database transactions).
   - `GameIt.Contracts` is shared between `GameIt.Application`, `GameIt.API`, and `GameIt.BlazorUI`.
2. **Security Boundaries:**
   - All catalog mutations (`POST`, `PUT`, `DELETE` on `Game`, `Category`, `Discount`) require `[Authorize(Roles = "Administrator")]`.
   - Purchase prices are calculated server-side in `CreatePurchaseCommandHandler`; client-supplied `AmountPaid` is never trusted.
   - Never commit secrets to `appsettings.json`; use `.env.example` placeholders and User Secrets / environment variables.
3. **Blazor UI Structure (`Core -> Shared -> Features`):**
   - `Core/`: `CustomAuthStateProvider`, `JwtAuthorizationMessageHandler`, `ProblemDetailsParser`, `ApiResult`, `ToastService`.
   - `Shared/Layout/` & `Shared/UI/`: `MainLayout`, `AuthLayout`, `Sidebar`, `TopBar`, and the 10 reusable UI primitives.
   - `Features/`: Vertical slices (`Auth`, `Catalog`, `Categories`, `Discounts`, `Reviews`, `Wishlist`, `Library`, `Purchases`, `Admin`).
   - Every view must implement all 4 UX states: **Loading (Skeleton)**, **Empty**, **Error**, and **Populated**.
