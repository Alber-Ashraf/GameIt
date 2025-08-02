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

    // Get: api/payment/GetPurchaseHistory/{userId}
    [HttpGet("users/{userId}/purchases")]
    [ProducesResponseType(typeof(List<PurchaseListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PurchaseListDto>>> GetPurchaseHistory(
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var query = new GetUserPurchaseListQuery();
        var purchases = await _mediator.Send(query, token);
        return Ok(purchases);
    }

    // Post: api/payment/CreatePurchase
    [HttpPost("purchases")]
    [ProducesResponseType(typeof(CreatePurchaseCommand), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CreatePurchaseCommand command,
        CancellationToken token = default)
    {
        var result = await _mediator.Send(command, token);
        return Ok(result);
    }
}
