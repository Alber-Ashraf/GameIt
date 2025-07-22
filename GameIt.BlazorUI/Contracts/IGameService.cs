using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Contracts;

public interface IGameService
{
    Task<GameVM> GetByIdWithDetailsAsync(Guid id, CancellationToken token = default);
    Task<List<GameVM>> GetAllWithCategoryAsync(CancellationToken token = default);
    Task<List<GameVM>> GetFeaturedGamesAsync(int count = 5, CancellationToken token = default);
    Task<List<GameVM>> GetGamesByCategoryAsync(Guid categoryId, int limit = 10, CancellationToken token = default);
    Task<List<GameVM>> GetSimilarGamesAsync(Guid gameId, int limit = 5, CancellationToken token = default);
    Task<Response<Guid>> CreateAsync(GameVM game);
    Task<Response<Guid>> Update(int id, GameVM game);
    Task<Response<Guid>> Delete(int id);
}
