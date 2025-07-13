using GameIt.Application.Interfaces.Persistence;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameItDbContext _context;
    private bool _disposed;

    public UnitOfWork(GameItDbContext context)
    {
        _context = context;
    }

    public IGameRepository Games => new GameRepository(_context);
    public IWishlistRepository Wishlists => new WishlistRepository(_context);
    public IDiscountRepository Discounts => new DiscountRepository(_context);
    public IReviewRepository Reviews => new ReviewRepository(_context);
    public ICommentRepository Comments => new CommentRepository(_context);
    public ICategoryRepository Categories => new CategoryRepository(_context);
    public IPurchaseRepository Purchases => new PurchaseRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
