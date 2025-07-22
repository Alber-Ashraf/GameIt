using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Contracts;

public interface IGameService
{
    Task<GameDetailsVM> GetByIdWithDetailsAsync(Guid id, CancellationToken token = default);
    Task<List<GamesListVM>> GetAllWithCategoryAsync(CancellationToken token = default);
    Task<List<GameDetailsVM>> GetFeaturedGamesAsync(int count = 5, CancellationToken token = default);
    Task<List<GameDetailsVM>> GetGamesByCategoryAsync(Guid categoryId, int limit = 10, CancellationToken token = default);
    Task<List<GameDetailsVM>> GetSimilarGamesAsync(Guid gameId, int limit = 5, CancellationToken token = default);
    Task<Response<Guid>> CreateAsync(GameDetailsVM game);
    Task<Response<Guid>> Update(Guid id, GameDetailsVM game);
    Task<Response<Guid>> Delete(Guid id);
}
