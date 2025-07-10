using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(GameItDbContext context) : base(context) { }
    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}
