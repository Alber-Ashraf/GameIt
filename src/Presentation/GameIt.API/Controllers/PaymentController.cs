using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Application.Features.Purchase.Queries.GetUserPurchase;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/payment/purchases
    [HttpGet("purchases")]
    [ProducesResponseType(typeof(List<PurchaseListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PurchaseListDto>>> GetMyPurchases(
        CancellationToken token = default)
    {
        var query = new GetUserPurchaseListQuery();
        var purchases = await _mediator.Send(query, token);
        return Ok(purchases);
    }

    // GET: api/payment/users/{userId}/purchases
    [HttpGet("users/{userId}/purchases")]
    [ProducesResponseType(typeof(List<PurchaseListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PurchaseListDto>>> GetPurchaseHistory(
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var query = new GetUserPurchaseListQuery();
        var purchases = await _mediator.Send(query, token);
        return Ok(purchases);
    }

    // POST: api/payment/purchases
    [HttpPost("purchases")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CreatePurchaseCommand command,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(command, token);
        return Ok(result);
    }
}
