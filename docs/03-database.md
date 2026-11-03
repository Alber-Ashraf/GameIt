Last updated: 2026-11-03

# 03 — Database Schema & Persistence Architecture

## 1. Entity-Relationship Diagram

`GameIt` uses SQL Server (`db_GameIt`) with two EF Core `DbContext` classes sharing the database:
- **`GameItDbContext` (`GameIt.Persistence`):** Manages `Games`, `Categories`, `Discounts`, `Reviews`, `Purchases`, `Wishlists`, and `Libraries`.
- **`GameItIdentityDbContext` (`GameIt.Identity`):** Manages ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, etc.).

```mermaid
erDiagram
    Categories ||--o{ Games : "categorizes (Restrict)"
    Games ||--o| Discounts : "has active (Cascade)"
    Games ||--o{ Reviews : "receives (Cascade)"
    Games ||--o{ Purchases : "purchased in (Restrict)"
    Games ||--o{ Wishlists : "wishlisted in (Cascade)"
    Games ||--o{ Libraries : "owned in (Cascade)"
    AspNetUsers ||--o{ Reviews : "writes (UserId)"
    AspNetUsers ||--o{ Purchases : "places (UserId)"
    AspNetUsers ||--o{ Wishlists : "maintains (UserId)"
    AspNetUsers ||--o{ Libraries : "owns (UserId)"

    Categories {
        uniqueidentifier Id PK
        nvarchar(100) CategoryName "Indexed (IX_Categories_Name)"
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Games {
        uniqueidentifier Id PK
        nvarchar(100) Name "Required"
        nvarchar(1000) Description
        nvarchar(300) ImageUrl
        decimal(10_2) Price
        bit IsFree "Default 0"
        bit IsFeatured "Default 0"
        bigint FileSizeInMB
        nvarchar(500) DownloadLink
        nvarchar(500) AdditionalDownloadLink
        nvarchar(max) SystemRequirements
        datetime2 ReleaseDate
        uniqueidentifier CategoryId FK
        nvarchar(max) PublisherId
        float AverageRating
        int TotalReviews
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Discounts {
        uniqueidentifier Id PK
        uniqueidentifier GameId FK "Unique 1:1"
        decimal(5_2) Percentage "CHECK 0..100"
        datetime2 StartDate "CHECK StartDate < EndDate"
        datetime2 EndDate
        bit IsActive "Default 1"
        nvarchar(500) Description
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Reviews {
        uniqueidentifier Id PK
        uniqueidentifier GameId FK
        nvarchar(450) UserId "Unique (UserId, GameId)"
        int Rating "CHECK 1..5"
        nvarchar(1000) Comment
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Purchases {
        uniqueidentifier Id PK
        uniqueidentifier GameId FK
        nvarchar(max) UserId
        datetime2 PurchaseDate
        decimal(18_2) AmountPaid
        decimal(18_2) OriginalPrice
        nvarchar(max) StripePaymentIntentId
        nvarchar(20) Status "Pending | Succeeded | Failed"
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Wishlists {
        nvarchar(450) UserId PK
        uniqueidentifier GameId PK_FK
        uniqueidentifier Id
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Libraries {
        uniqueidentifier Id PK
        nvarchar(450) UserId "Required"
        uniqueidentifier GameId FK
        datetime2 PurchasedAt
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }
```

## 2. Constraints & Indexes

| Table | Constraint / Index | Definition | Purpose |
| :--- | :--- | :--- | :--- |
| `Categories` | `IX_Categories_Name` | Index on `CategoryName` | Fast category lookup by name |
| `Discounts` | `IX_Discounts_GameId` | Unique Index on `GameId` | Enforces 1:1 active discount per game |
| `Discounts` | `CK_Discounts_Percentage` | `[Percentage] BETWEEN 0 AND 100` | Prevents negative or >100% discounts |
| `Discounts` | `CK_Discounts_DateRange` | `[StartDate] < [EndDate]` | Enforces valid promotion window |
| `Reviews` | `IX_Reviews_UserId_GameId` | Unique Index on `(UserId, GameId)` | One review per user per game |
| `Reviews` | `CK_Reviews_Rating` | `[Rating] BETWEEN 1 AND 5` | Enforces 1–5 star rating scale |
| `Wishlists` | `PK_Wishlists` | Composite PK `(UserId, GameId)` | Prevents duplicate wishlist entries |

## 3. Persistence Hardening Plan (Phase 3)
1. **UTC Timestamps:** Standardize `GameItDbContext.SaveChangesAsync` to set `UpdatedAt = DateTime.UtcNow` and `CreatedAt = DateTime.UtcNow`.
2. **Cached Repositories in `UnitOfWork`:** Lazily cache repository instances per `UnitOfWork` scope (`_games ??= new GameRepository(_context)`) instead of allocating new instances on every property read.
3. **Eager Loading & Null Safety:**
   - Include `Discount` and `Category` consistently across `GetFeaturedGamesAsync`, `GetGamesByCategoryAsync`, and `GetSimilarGamesAsync`.
   - Guard `GetSimilarGamesAsync` when `baseGame == null` (return empty list instead of throwing `NullReferenceException`).
   - Populate `GameName` and `CategoryName` in `WishlistListDto` projection (`WishlistRepository.GetUserWishlistWithDetailsAsync`).

## 4. Automated Database Seeder (`GameItDatabaseSeeder`)
GameIt includes an idempotent, automated seeder in `GameIt.Persistence.DatabaseContext.GameItDatabaseSeeder` executed on API startup when `Database:SeedDataOnStartup` is enabled (default `true`):
- **10 Core Categories:** Action, RPG, Adventure, Strategy, Shooter, Open World, Indie, Sports & Racing, Horror, Simulation.
- **50 Popular Games:** Spans major PC titles (Cyberpunk 2077, Elden Ring, Baldur's Gate 3, Red Dead Redemption 2, GTA V, Black Myth: Wukong, Helldivers 2, etc.) with high-resolution Steam CDN header art, accurate pricing and free-to-play flags, system requirements, file sizes, and empty `DownloadLink` values.
- **10 Active Discounts:** 20% to 75% active discounts on select flagship titles for storefront deals and discount badges.

