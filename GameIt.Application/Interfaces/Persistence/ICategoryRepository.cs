using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken token = default);
    Task<bool> IsCategoryUniqueForCreate(string name, CancellationToken token = default);
    Task<bool> IsCategoryNameUniqueForUpdate(Guid id, string name, CancellationToken token = default);
}
