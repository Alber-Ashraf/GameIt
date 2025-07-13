using GameIt.Domain.Common;

namespace GameIt.Application.Interfaces.Persistence;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task CreateAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Guid id, CancellationToken token = default);
}
