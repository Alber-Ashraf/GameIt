using GameIt.Application.Features.Category.Commands.CreateCategory;
using GameIt.Application.Features.Category.Commands.DeleteCategory;
using GameIt.Application.Features.Category.Commands.UpdateCategory;
using GameIt.Application.Features.Category.Queries.GetAllCategoryLists;
using GameIt.Application.Features.Category.Queries.GetAllGameLists;
using GameIt.Application.Features.Category.Queries.GetCategoryByName;
using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Category
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CategoriesListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoriesListDto>>> GetCategorys(
        CancellationToken token)
    {
        var query = new GetAllCategoriesListQuery();
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // GET: api/Category/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDetailsDto>> GetCategoryById(
        [FromRoute] Guid id,
        CancellationToken token)
    {
        var query = new GetCategoryDetailsQuery(id);
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // GET: api/Category/name/{name} or api/Category/by-name/{name}
    [HttpGet("name/{name}")]
    [HttpGet("by-name/{name}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDetailsDto>> GetCategoryByName(
        [FromRoute] string name,
        CancellationToken token)
    {
        var query = new GetCategoryByNameQuery(name);
        var result = await _mediator.Send(query, token);
        return Ok(result);
    }

    // POST: api/Category
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken token)
    {
        var result = await _mediator.Send(command, token);
        return CreatedAtAction(
            nameof(GetCategoryById),
            new { id = result }, result);
    }

    // PUT: api/Category/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken token)
    {
        if (id != command.Id) return BadRequest("Route ID and body ID mismatch");

        await _mediator.Send(command, token);
        return NoContent();
    }

    // DELETE: api/Category/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(
        [FromRoute] Guid id,
        CancellationToken token)
    {
        var command = new DeleteCategoryCommand { Id = id };
        await _mediator.Send(command, token);
        return NoContent();
    }
}
