using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(GameItDbContext context) : base(context) { }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken token = default)
    {
        return await _dbSet
        .Where(c => c.Name.ToLower() == name.ToLower())
        .AsNoTracking()
        .FirstOrDefaultAsync(token);
    }

    public async Task<bool> IsCategoryNameUniqueForUpdate(Guid id, string name, CancellationToken token = default)
    {
        return !await _context.Categories
                    .AsNoTracking()
                    .AnyAsync(g => g.Id != id && g.Name == name);
    }

    public async Task<bool> IsCategoryUniqueForCreate(string name, CancellationToken token = default)
    {
        return !await _context.Categories
                    .AsNoTracking()
                    .AnyAsync(g => g.Name == name);
    }
}
