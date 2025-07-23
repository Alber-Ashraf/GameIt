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
using Microsoft.AspNetCore.Authorization;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get: api/game
    [HttpGet]
    [ProducesResponseType(typeof(List<GamesListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GamesListDto>>> GetGames(CancellationToken token = default)
    {
        var query = new GetAllGamesListQuery();
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // Get: api/game/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDetailsDto>> GetGameById([FromRoute] Guid id,
        CancellationToken token = default)
    {
        var query = new GetGameDetailsQuery(id);
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // Get: api/game/category/{categoryId}
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(List<GamesListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GamesListDto>>> GetGamesByCategory(
        [FromRoute] Guid categoryId,
        [FromQuery] int limit = 10,
        CancellationToken token = default)
    {
        var query = new GetGamesByCategoryQuery(categoryId, limit);
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // Get: api/game/featured
    [HttpGet("featured")]
    [ProducesResponseType(typeof(List<GamesListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GamesListDto>>> GetFeaturedGames(
        [FromQuery, Range(1, 20)] int limit = 5,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(new GetFeaturedGamesQuery(limit), token);
        return Ok(result);
    }

    // Get: api/game/similar/{gameId}
    [HttpGet("similar/{gameId:guid}")]
    [ProducesResponseType(typeof(List<GamesListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GamesListDto>>> GetSimilarGames(
        [FromRoute] Guid gameId,
        [FromQuery, Range(1, 20)] int limit = 5,
        CancellationToken token = default)
    {
        var query = new GetSimilarGamesQuery(gameId, limit);
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // Post: api/game
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGame(
        [FromBody] CreateGameCommand command,
        CancellationToken token = default)
    {
        var gameId = await _mediator.Send(command, token);
        return CreatedAtAction(nameof(GetGameById), new { id = gameId }, gameId);
    }

    // Put: api/game/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGame(
        [FromRoute] Guid id,
        [FromBody] UpdateGameCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return NoContent();
    }

    // Delete: api/game/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGame([FromRoute] Guid id,
        CancellationToken token)
    {
        var command = new DeleteGameCommand() { Id = id};
        await _mediator.Send(command, token);
        return NoContent();
    }
}
