using GameIt.Application.Features.Game.Queries.GetAllGameDetails;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Features.Game.Queries.GetGamesByCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get: api/game
    [HttpGet]
    public async Task<List<GamesListDto>> GetGames()
    {
        var query = new GetAllGamesListQuery();
        var result = await _mediator.Send(query);

        // If no games are found, return an empty list
        if (result == null || !result.Any())
        {
            return new List<GamesListDto>();
        }

        return result;
    }
}
