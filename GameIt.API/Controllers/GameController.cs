using System.ComponentModel.DataAnnotations;
using GameIt.Application.Features.Game.Queries.GetAllGameDetails;
using GameIt.Application.Features.Game.Queries.GetAllGameLists;
using GameIt.Application.Features.Game.Queries.GetFeaturedGames;
using GameIt.Application.Features.Game.Queries.GetGamesByCategory;
using GameIt.Application.Features.Game.Queries.GetSimilarGames;
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

    // Get: api/game/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameById([FromRoute] Guid id)
    {
        var query = new GetGameDetailsQuery(id);
        var result = await _mediator.Send(query);

        // If the game is not found, return a 404 Not Found response
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // Get: api/game/category/{categoryId}
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetGamesByCategory(
        [FromRoute] Guid categoryId,
        [FromQuery] int limit = 10)
    {
        var query = new GetGamesByCategoryQuery(categoryId, limit);
        var result = await _mediator.Send(query);

        // If no games are found for the specified category, return a 404 Not Found response
        if (!result.Any())
        {
            return NotFound($"No games found for category {categoryId}");
        }

        return Ok(result);
    }

    // Get: api/game/featured
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedGames(
    [FromQuery, Range(1, 20)] int limit = 5)
    {
        var result = await _mediator.Send(new GetFeaturedGamesQuery(limit));

        // If no featured games are found, return a 404 Not Found response
        if (!result.Any()) 
        {
            return NotFound("No featured games currently available");
        }

        return Ok(result);
    }

    // Get: api/game/similar/{gameId}
    [HttpGet("similar/{gameId}")]
    public async Task<IActionResult> GetSimilarGames(
    [FromRoute] Guid gameId,
    [FromQuery, Range(1, 20)] int limit = 5)
    {
        var query = new GetSimilarGamesQuery(gameId, limit);
        var result = await _mediator.Send(query);
        // If no similar games are found, return a 404 Not Found response
        if (!result.Any())
        {
            return NotFound($"No similar games found for game {gameId}");
        }
        return Ok(result);
    }
}
