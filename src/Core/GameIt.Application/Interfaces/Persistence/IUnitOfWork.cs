namespace GameIt.Application.Interfaces.Persistence;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork : IDisposable
{
    IGameRepository Games { get; }
    IWishlistRepository Wishlists { get; }
    IDiscountRepository Discounts { get; }
    IReviewRepository Reviews { get; }
    ICategoryRepository Categories { get; }
    IPurchaseRepository Purchases { get; }
    ILibraryRepository Libraries { get; }
    Task<ITransactionScope> BeginTransactionAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
