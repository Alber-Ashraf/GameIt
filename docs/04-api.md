Last updated: 2026-10-29

# 04 — REST API Specification & Security Contract

## 1. Authentication & Authorization Contract
- **Scheme:** `Bearer <JWT>` in `Authorization` HTTP header.
- **Claims:**
  - `uid`: User GUID (`ApplicationUser.Id`)
  - `sub`: Username
  - `email`: User email
  - `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`: Role (`Administrator`, `Employee`, `User`)
- **Default Registration Role:** `"User"` (fixed from `"Employee"`).

## 2. Error Response Format (`CustomProblemDetails`)
All `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, and `500 Internal Server Error` responses return RFC 7807 JSON:
```json
{
  "type": "BadRequestException",
  "title": "Invalid Game",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "Name": ["Game name is required."],
    "Price": ["Price must be 0 for free games."]
  }
}
```
*Security Rule:* In non-Development environments, `500 Internal Server Error` responses never include `ex.StackTrace` in `detail`.

## 3. Endpoints Matrix

### 3.1 Auth (`/api/Auth`)
| Method | Route | Auth | Request Body | Response |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/login` | Anonymous | `AuthRequest { Email, Password }` | `200 OK` -> `AuthResponse { Id, UserName, Email, Token }` |
| `POST` | `/api/Auth/register` | Anonymous | `RegistrationRequest { FirstName, LastName, Email, UserName, Password, ProfilePictureUrl? }` | `200 OK` -> `RegistrationResponse { UserId }` |

### 3.2 Games (`/api/Game`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Game` | Anonymous | — | `200 OK` -> `List<GamesListDto>` (returns `[]` if empty) |
| `GET` | `/api/Game/{id:guid}` | Anonymous | `id` | `200 OK` -> `GameDetailsDto` / `404` |
| `GET` | `/api/Game/featured` | Anonymous | `?limit=5` (1..20) | `200 OK` -> `List<GamesListDto>` |
| `GET` | `/api/Game/category/{categoryId:guid}` | Anonymous | `?limit=10` | `200 OK` -> `List<GamesListDto>` |
| `GET` | `/api/Game/similar/{gameId:guid}` | Anonymous | `?limit=5` (1..20) | `200 OK` -> `List<GamesListDto>` |
| `POST` | `/api/Game` | `Administrator` | `CreateGameCommand` | `201 Created` -> `Guid` |
| `PUT` | `/api/Game/{id:guid}` | `Administrator` | `UpdateGameCommand` | `204 No Content` |
| `DELETE` | `/api/Game/{id:guid}` | `Administrator` | `id` | `204 No Content` |

### 3.3 Categories (`/api/Category`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Category` | Anonymous | — | `200 OK` -> `List<CategoriesListDto>` |
| `GET` | `/api/Category/{id:guid}` | Anonymous | `id` | `200 OK` -> `CategoryDetailsDto` |
| `GET` | `/api/Category/name/{name}` | Anonymous | `name` | `200 OK` -> `CategoryDetailsDto` |
| `POST` | `/api/Category` | `Administrator` | `CreateCategoryCommand` | `201 Created` -> `Guid` |
| `PUT` | `/api/Category/{id:guid}` | `Administrator` | `UpdateCategoryCommand` | `204 No Content` |
| `DELETE` | `/api/Category/{id:guid}` | `Administrator` | `id` | `204 No Content` |

### 3.4 Discounts (`/api/Discount`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Discount` | Anonymous | — | `200 OK` -> `List<ActiveDiscountDto>` |
| `POST` | `/api/Discount` | `Administrator` | `CreateDiscountCommand` | `201 Created` -> `Guid` |
| `PUT` | `/api/Discount/{id:guid}` | `Administrator` | `UpdateDiscountCommand` | `204 No Content` |
| `DELETE` | `/api/Discount/{id:guid}` | `Administrator` | `id` | `204 No Content` |

### 3.5 Reviews (`/api/Review`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Review/reviews/{gameId:guid}` | Anonymous (`[AllowAnonymous]`) | `gameId` | `200 OK` -> `List<ReviewListDto>` |
| `POST` | `/api/Review/{gameId:guid}` | Authenticated | `CreateReviewCommand` | `201 Created` -> `{ ReviewId }` |
| `PUT` | `/api/Review/{id:guid}/{gameId:guid}` | Authenticated (Owner) | `UpdateReviewCommand` | `204 No Content` |
| `DELETE` | `/api/Review/{id:guid}` | Authenticated (Owner or `Administrator`) | `id` | `204 No Content` |

### 3.6 Wishlist (`/api/Wishlist`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Wishlist` (and `/api/Wishlist/users/{userId}`) | Authenticated | Current User (`uid`) | `200 OK` -> `List<WishlistListDto>` |
| `POST` | `/api/Wishlist/items` | Authenticated | `AddToWishlistCommand { GameId }` | `201 Created` -> `Guid` |
| `DELETE` | `/api/Wishlist/items/{gameId:guid}` | Authenticated | `gameId` | `204 No Content` |
| `DELETE` | `/api/Wishlist` | Authenticated | — | `204 No Content` |

### 3.7 Purchases & Payments (`/api/Payment` & `/api/webhook`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Payment/purchases` (and `/api/Payment/users/{userId}/purchases`) | Authenticated | Current User (`uid`) | `200 OK` -> `List<PurchaseListDto>` |
| `POST` | `/api/Payment/purchases` | Authenticated | `CreatePurchaseCommand { GameId, AmountPaid }` | `200 OK` -> `PurchaseResult { Id, Status, StripeCheckoutUrl, PurchaseDate }` |
| `POST` | `/api/webhook` | Stripe Signature | Raw JSON + `Stripe-Signature` | `200 OK` |

### 3.8 Library (`/api/Library`)
| Method | Route | Auth | Request / Params | Response |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Library` (and `/api/Library/users/{userId}`) | Authenticated | Current User (`uid`) | `200 OK` -> `List<LibraryListDto>` |
