namespace GameIt.Application.Interfaces.Persistence;

public interface IUnitOfWork
{
    IGameRepository Games { get; }
    IWishlistRepository Wishlists { get; }
    IDiscountRepository Discounts { get; }
    IReviewRepository Reviews { get; }
    ICategoryRepository Categories { get; }
    IPurchaseRepository Purchases { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
