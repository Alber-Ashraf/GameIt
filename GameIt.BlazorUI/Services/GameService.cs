using AutoMapper;
using Blazored.LocalStorage;
using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class GameService : BaseHttpService, IGameService
{
    private readonly IMapper _mapper;
    public GameService(IClient client, IMapper mapper, ILocalStorageService localStorage) : base(client, localStorage) 
    {
        _mapper = mapper;
    }

    public async Task<Response<Guid>> CreateAsync(GameDetailsVM game)
    {
        try
        {
            var command = _mapper.Map<CreateGameCommand>(game);
            await _client.GamePOSTAsync(command);
            return new Response<Guid>()
            {
                Success = true,
                Message = "Game created successfully.",
                Data = game.Id
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptions<Guid>(ex);
        }
    }

    public async Task<Response<Guid>> Delete(Guid id)
    {
        try
        {
            await _client.GameDELETEAsync(id);
            return new Response<Guid>()
            {
                Success = true,
                Message = "Game deleted successfully.",
                Data = id
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptions<Guid>(ex);
        }
    }

    public async Task<List<GamesListVM>> GetAllWithCategoryAsync(CancellationToken token = default)
    {
        var games = await _client.GameAllAsync();
        return _mapper.Map<List<GamesListVM>>(games);
    }

    public async Task<GameDetailsVM> GetByIdWithDetailsAsync(Guid id, CancellationToken token = default)
    {
        var game = await _client.GameGETAsync(id, token);
        return _mapper.Map<GameDetailsVM>(game);
    }

    public async Task<List<GamesListVM>> GetFeaturedGamesAsync(int count = 5, CancellationToken token = default)
    {
        var games = await _client.FeaturedAsync(count, token);
        return _mapper.Map<List<GamesListVM>>(games);
    }

    public async Task<List<GamesListVM>> GetGamesByCategoryAsync(Guid categoryId, int limit = 10, CancellationToken token = default)
    {
        var games = await _client.CategoryAsync(categoryId, limit, token);
        return _mapper.Map<List<GamesListVM>>(games);
    }

    public async Task<List<GamesListVM>> GetSimilarGamesAsync(Guid gameId, int limit = 5, CancellationToken token = default)
    {
        var games = await _client.SimilarAsync(gameId, limit, token);
        return _mapper.Map<List<GamesListVM>>(games);
    }

    public async Task<Response<Guid>> Update(Guid id, GameDetailsVM game)
    {
        try
        {
            var command = _mapper.Map<UpdateGameCommand>(game);
            await _client.GamePUTAsync(id, command);
            return new Response<Guid>()
            {
                Success = true,
                Message = "Game updated successfully.",
                Data = id
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptions<Guid>(ex);
        }
    }
}