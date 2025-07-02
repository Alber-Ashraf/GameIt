using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameIt.Domain;

namespace GameIt.Application.Interfaces.Persistence
{
    public interface IGameRepository : IGenericRepository<Game>
    {
        Task<Game> GetByIdWithDetailsAsync(Guid id);
        Task<List<Game>> GetAllWithCategoryAsync();
    }
}
