using GameIt.Application.Features.Wishlist.Commands.AddToWishlist;
using GameIt.Application.Features.Wishlist.Commands.ClearWishlist;
using GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;
    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get: api/wishlist/{userId}
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(List<WishlistListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<WishlistListDto>>> GetWishlistByUser(
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var query = new GetUserWishlistListQuery(userId);
        var wishlist = await _mediator.Send(query, token);
        return Ok(wishlist);
    }

    // Post: api/wishlist
    [HttpPost("items")]
    [ProducesResponseType(typeof(AddToWishlistCommand), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistCommand command,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(
                nameof(GetWishlistByUser),
                new { userId = command.UserId });
    }

    // Delete: api/wishlist/{gameId}/{userId}
    [HttpDelete("items/{gameId:guid}/users/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromWishlist(
        [FromRoute] Guid gameId,
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var command = new RemoveFromWishlistCommand { GameId = gameId, UserId = userId };
        await _mediator.Send(command, token);
        return NoContent();
    }

    // Delete: api/wishlist/{userId}
    [HttpDelete("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearWishlist(
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var command = new ClearWishlistCommand { UserId = userId };
        await _mediator.Send(command, token);
        return NoContent();
    }
}
