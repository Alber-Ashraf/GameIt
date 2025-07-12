using System.ComponentModel.DataAnnotations;
using GameIt.Application.Features.Game.Commands.CreateGame;
using GameIt.Application.Features.Game.Commands.DeleteGame;
using GameIt.Application.Features.Game.Commands.UpdateGame;
using GameIt.Application.Features.Game.Queries.GetGameDetails;
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
    public async Task<IActionResult> GetGames()
    {
        var query = new GetAllGamesListQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Get: api/game/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGameById([FromRoute] Guid id)
    {
        var query = new GetGameDetailsQuery(id);
        var result = await _mediator.Send(query);
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
        return Ok(result);
    }

    // Get: api/game/featured
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedGames(
    [FromQuery, Range(1, 20)] int limit = 5)
    {
        var result = await _mediator.Send(new GetFeaturedGamesQuery(limit));
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
        return Ok(result);
    }

    // Post: api/game
    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameCommand command)
    {
        var gameId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetGameById), new { id = gameId }, gameId);
    }

    // Put: api/game/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGame(
        [FromRoute] Guid id,
        [FromBody] UpdateGameCommand command)
    {
        var result = await _mediator.Send(command);
        return NoContent();
    }

    // Delete: api/game/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame([FromRoute] Guid id)
    {
        var command = new DeleteGameCommand() { Id = id};
        await _mediator.Send(command);
        return NoContent();
    }
}
