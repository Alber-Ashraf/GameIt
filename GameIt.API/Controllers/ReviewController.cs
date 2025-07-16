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
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetReviewsByGame(
        [FromRoute] Guid gameId,
        CancellationToken token)
    {
        var query = new GetReviewsByGameQuery(gameId);
        var reviews = await _mediator.Send(query, token);
        return Ok(reviews);
    }

    // Post: api/review
    [HttpPost]
    public async Task<IActionResult> CreateReview(
        [FromBody] CreateReviewCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(nameof(CreateReview), new { id = result }, result);
    }

    //Update: api/review/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(
        [FromRoute] Guid id,
        [FromBody] UpdateReviewCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return NoContent();
    }

    // Delete: api/review/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(
        [FromRoute] Guid id,
        CancellationToken token)
    {
        var command = new DeleteReviewCommand() { Id = id};
        await _mediator.Send(command, token);
        return NoContent();
    }
}
