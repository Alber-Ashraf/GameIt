using GameIt.Application.Features.Review.Commands.CreateReview;
using GameIt.Application.Features.Review.Commands.DeleteReview;
using GameIt.Application.Features.Review.Commands.UpdateReview;
using GameIt.Application.Features.Review.Queries.GetReviewsByGame;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }
    // Get: api/review/{gameId}
    [HttpGet("reviews/{gameId:guid}")]
    [ProducesResponseType(typeof(List<ReviewListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ReviewListDto>>> GetReviewsByGame(
        [FromRoute] Guid gameId,
        CancellationToken token = default)
    {
        var query = new GetReviewsByGameQuery(gameId);
        var reviews = await _mediator.Send(query, token);
        return Ok(reviews);
    }

    // Post: api/review
    [HttpPost]
    [ProducesResponseType(typeof(CreateReviewCommand), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateReview(
        [FromBody] CreateReviewCommand command,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(
                    nameof(GetReviewsByGame),
                    new { gameId = command.GameId });
    }

    //Update: api/review/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(
        [FromRoute] Guid id,
        [FromBody] UpdateReviewCommand command,
        CancellationToken token = default)
    {
        if (id != command.Id)
            return BadRequest("Review ID mismatch.");

        var result = await _mediator.Send(command, token);
        return NoContent();
    }

    // Delete: api/review/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var command = new DeleteReviewCommand() { Id = id};
        await _mediator.Send(command, token);
        return NoContent();
    }
}
