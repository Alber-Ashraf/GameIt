using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameIt.Application.Interfaces.Persistence
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
