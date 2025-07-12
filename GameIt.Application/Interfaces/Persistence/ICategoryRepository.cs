using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}
