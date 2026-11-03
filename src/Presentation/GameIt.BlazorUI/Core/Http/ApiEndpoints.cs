namespace GameIt.BlazorUI.Core.Http;

public static class ApiEndpoints
{
    public const string AuthLogin = "api/auth/login";
    public const string AuthRegister = "api/auth/register";

    public const string Games = "api/game";
    public const string GamesFeatured = "api/game/featured";
    public static string GameById(Guid id) => $"api/game/{id}";
    public static string GamesByCategory(Guid categoryId, int limit = 20) => $"api/game/category/{categoryId}?limit={limit}";
    public static string GamesSimilar(Guid gameId, int limit = 5) => $"api/game/similar/{gameId}?limit={limit}";

    public const string Categories = "api/category";
    public static string CategoryById(Guid id) => $"api/category/{id}";
    public static string CategoryByName(string name) => $"api/category/by-name/{Uri.EscapeDataString(name)}";

    public const string Discounts = "api/discount";
    public const string DiscountsActive = "api/discount/active";
    public static string DiscountById(Guid id) => $"api/discount/{id}";

    public const string Reviews = "api/review";
    public static string ReviewsByGame(Guid gameId) => $"api/review/game/{gameId}";
    public static string ReviewById(Guid id) => $"api/review/{id}";

    public const string Wishlist = "api/wishlist";
    public const string WishlistItems = "api/wishlist/items";
    public static string WishlistByUser(string userId) => $"api/wishlist/users/{Uri.EscapeDataString(userId)}";
    public static string WishlistItemByGame(Guid gameId) => $"api/wishlist/items/{gameId}";

    public const string Library = "api/library";
    public static string LibraryByUser(string userId) => $"api/library/users/{Uri.EscapeDataString(userId)}";

    public const string Purchases = "api/payment/purchases";
    public static string PurchasesByUser(string userId) => $"api/payment/users/{Uri.EscapeDataString(userId)}/purchases";
    public static string ConfirmPaymentSession(string sessionId) => $"api/payment/confirm-session?sessionId={Uri.EscapeDataString(sessionId)}";
    public static string VerifyPurchase(Guid purchaseId) => $"api/payment/purchases/{purchaseId}/verify";
}
