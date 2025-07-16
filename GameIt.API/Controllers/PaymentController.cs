using GameIt.Application.Features.Purchase.Commands.CreatePurchase;
using GameIt.Application.Features.Purchase.Commands.RefundPurchase;
using GameIt.Application.Features.Purchase.Queries.GetUserPurchase;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Post: api/payment/CreatePurchase
    [HttpPost("CreatePurchase")]
    public async Task<IActionResult> CreatePurchase(
        [FromBody] CreatePurchaseCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(nameof(CreatePurchase), new { id = result.Id }, result);
    }

    // Post: api/payment/RefundPurchase
    [HttpPost("RefundPurchase")]
    public async Task<IActionResult> RefundPurchase(
        [FromBody] RefundPurchaseCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return Ok(result);
    }

    // Get: api/payment/GetPurchaseHistory/{userId}
    [HttpGet("GetPurchaseHistory/{userId}")]
    public async Task<IActionResult> GetPurchaseHistory(
        [FromRoute] string userId,
        CancellationToken token)
    {
        var query = new GetUserPurchaseListQuery(userId);
        var purchases = await _mediator.Send(query, token);
        return Ok(purchases);
    }
}
