using GameIt.Application.Features.Discount.Commands.CreateDiscount;
using GameIt.Application.Features.Discount.Commands.DeleteDiscount;
using GameIt.Application.Features.Discount.Commands.UpdateDiscount;
using GameIt.Application.Features.Discount.Queries.GetActiveDiscounts;
using GameIt.Application.Features.Game.Commands.DeleteGame;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountController : ControllerBase
{
    private readonly IMediator _mediator;
    public DiscountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Discount
    [HttpGet]
    public async Task<IActionResult> GetActiveDiscounts(
        CancellationToken token)
    {
        var discounts = await _mediator.Send(new GetActiveDiscountsQuery(), token);
        return Ok(discounts);
    }

    // POST: api/Discount
    [HttpPost]
    public async Task<IActionResult> CreateDiscount(
        [FromBody] CreateDiscountCommand command,
        CancellationToken token)
    {
        var discountId = await _mediator.Send(command, token);
        return Created($"api/discounts/{discountId}", null); ;
    }

    // PUT: api/Discount/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscount(
        [FromRoute] Guid id,
        [FromBody] UpdateDiscountCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return Ok();
    }

    // DELETE: api/Discount/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscount(
        [FromRoute] Guid id,
        CancellationToken token)
    {
        var command = new DeleteDiscountCommand() { Id = id };
        await _mediator.Send(command, token);
        return Ok();
    }
}
