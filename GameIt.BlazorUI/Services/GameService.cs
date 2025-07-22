using AutoMapper;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class GameService : BaseHttpService, IGameService
{
    private readonly IMapper _mapper;
    public GameService(IClient client, IMapper mapper) : base(client) 
    {
        _mapper = mapper;
    }

    public Task<Response<Guid>> CreateAsync(GameVM game)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Guid>> Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GameVM>> GetAllWithCategoryAsync(CancellationToken token = default)
    {
        var games = _client.GameGETAsync(token);
        return _mapper.Map<List<GameVM>>(games);
    }

    public Task<GameVM> GetByIdWithDetailsAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<GameVM>> GetFeaturedGamesAsync(int count = 5, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<GameVM>> GetGamesByCategoryAsync(Guid categoryId, int limit = 10, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<GameVM>> GetSimilarGamesAsync(Guid gameId, int limit = 5, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Guid>> Update(int id, GameVM game)
    {
        throw new NotImplementedException();
    }
}