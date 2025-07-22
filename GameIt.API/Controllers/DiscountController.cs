using GameIt.Application.Features.Discount.Commands.CreateDiscount;
using GameIt.Application.Features.Discount.Commands.DeleteDiscount;
using GameIt.Application.Features.Discount.Commands.UpdateDiscount;
using GameIt.Application.Features.Discount.Queries.GetActiveDiscounts;
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
    [ProducesResponseType(typeof(List<ActiveDiscountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActiveDiscountDto>>> GetActiveDiscounts(
        CancellationToken token = default)
    {
        var query = new GetActiveDiscountsQuery();
        var discounts = await _mediator.Send(query, token);
        return Ok(discounts);
    }

    // POST: api/Discount
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDiscount(
        [FromBody] CreateDiscountCommand command,
        CancellationToken token = default)
    {
        var discountId = await _mediator.Send(command, token);
        return CreatedAtAction(
            nameof(GetActiveDiscounts), 
            new { gameId = command.GameId });
    }

    // PUT: api/Discount/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDiscount(
        [FromRoute] Guid id,
        [FromBody] UpdateDiscountCommand command,
        CancellationToken token = default)
    {
        if (id != command.Id)
            return BadRequest("Discount ID mismatch.");

        var result = await _mediator.Send(command, token);
        return NoContent();
    }

    // DELETE: api/Discount/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDiscount(
        [FromRoute] Guid id,
        CancellationToken token = default)
    {
        var command = new DeleteDiscountCommand() { Id = id };
        await _mediator.Send(command, token);
        return NoContent();
    }
}
