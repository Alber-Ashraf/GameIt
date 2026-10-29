using GameIt.Application.Interfaces.Persistence;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace GameIt.Persistence.Repositories;

internal sealed class EfTransactionScope : ITransactionScope
{
    private readonly IDbContextTransaction _transaction;

    public EfTransactionScope(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => _transaction.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default)
        => _transaction.RollbackAsync(cancellationToken);

    public void Dispose() => _transaction.Dispose();

    public ValueTask DisposeAsync() => _transaction.DisposeAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly GameItDbContext _context;
    private bool _disposed;

    private IGameRepository? _games;
    private IWishlistRepository? _wishlists;
    private IDiscountRepository? _discounts;
    private IReviewRepository? _reviews;
    private ICategoryRepository? _categories;
    private IPurchaseRepository? _purchases;
    private ILibraryRepository? _libraries;

    public UnitOfWork(GameItDbContext context)
    {
        _context = context;
    }

    public IGameRepository Games => _games ??= new GameRepository(_context);
    public IWishlistRepository Wishlists => _wishlists ??= new WishlistRepository(_context);
    public IDiscountRepository Discounts => _discounts ??= new DiscountRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IPurchaseRepository Purchases => _purchases ??= new PurchaseRepository(_context);
    public ILibraryRepository Libraries => _libraries ??= new LibraryRepository(_context);

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

    public async Task<ITransactionScope> BeginTransactionAsync()
    {
        var tx = await _context.Database.BeginTransactionAsync();
        return new EfTransactionScope(tx);
    }
}
