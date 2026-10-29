using GameIt.Application.Features.Wishlist.Commands.AddToWishlist;
using GameIt.Application.Features.Wishlist.Commands.ClearWishlist;
using GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/wishlist
    [HttpGet]
    [ProducesResponseType(typeof(List<WishlistListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WishlistListDto>>> GetWishlist(
        CancellationToken token = default)
    {
        var query = new GetUserWishlistListQuery();
        var wishlist = await _mediator.Send(query, token);
        return Ok(wishlist);
    }

    // GET: api/wishlist/users/{userId}
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(List<WishlistListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WishlistListDto>>> GetWishlistByUser(
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var query = new GetUserWishlistListQuery();
        var wishlist = await _mediator.Send(query, token);
        return Ok(wishlist);
    }

    // POST: api/wishlist/items
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistCommand command,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(command, token);
        return Created(string.Empty, new { id = result });
    }

    // DELETE: api/wishlist/items/{gameId}
    [HttpDelete("items/{gameId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromWishlist(
        [FromRoute] Guid gameId,
        CancellationToken token = default)
    {
        var command = new RemoveFromWishlistCommand { GameId = gameId };
        await _mediator.Send(command, token);
        return NoContent();
    }

    // DELETE: api/wishlist
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ClearWishlist(
        CancellationToken token = default)
    {
        var command = new ClearWishlistCommand();
        await _mediator.Send(command, token);
        return NoContent();
    }
}
