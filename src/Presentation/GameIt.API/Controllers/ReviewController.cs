using GameIt.Application.Features.Review.Commands.CreateReview;
using GameIt.Application.Features.Review.Commands.DeleteReview;
using GameIt.Application.Features.Review.Commands.UpdateReview;
using GameIt.Application.Features.Review.Queries.GetReviewsByGame;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/review/reviews/{gameId} or api/review/game/{gameId}
    [HttpGet("reviews/{gameId:guid}")]
    [HttpGet("game/{gameId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ReviewListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewListDto>>> GetReviewsByGame(
        [FromRoute] Guid gameId,
        CancellationToken token = default)
    {
        var query = new GetReviewsByGameQuery(gameId);
        var reviews = await _mediator.Send(query, token);
        return Ok(reviews);
    }

    // POST: api/review
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReviewFromBody(
        [FromBody] CreateReviewCommand command,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(
            nameof(GetReviewsByGame),
            new { gameId = command.GameId },
            result);
    }

    // POST: api/review/{gameId}
    [HttpPost("{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReview(
        [FromRoute] Guid gameId,
        [FromBody] CreateReviewCommand command,
        CancellationToken token = default)
    {
        command.GameId = gameId;
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(
            nameof(GetReviewsByGame),
            new { gameId = gameId },
            new { ReviewId = result });
    }

    // PUT: api/review/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReviewById(
        [FromRoute] Guid id,
        [FromBody] UpdateReviewCommand command,
        CancellationToken token = default)
    {
        command.Id = id;
        await _mediator.Send(command, token);
        return NoContent();
    }

    // PUT: api/review/{id}/{gameId}
    [HttpPut("{id:guid}/{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(
        [FromRoute] Guid id,
        [FromRoute] Guid gameId,
        [FromBody] UpdateReviewCommand command,
        CancellationToken token = default)
    {
        if (command.Id != Guid.Empty && id != command.Id)
            return BadRequest("Review ID mismatch.");
        command.Id = id;
        command.GameId = gameId;

        await _mediator.Send(command, token);
        return NoContent();
    }

    // DELETE: api/review/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var command = new DeleteReviewCommand { Id = id };
        await _mediator.Send(command, token);
        return NoContent();
    }
}
