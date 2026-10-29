using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain.Common;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly GameItDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(GameItDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        // Use AsNoTracking for better performance when not tracking changes
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        // Use AsNoTracking for better performance when not tracking changes
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken token = default)
    {
        return await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id);
    }
}