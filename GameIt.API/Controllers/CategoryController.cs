using GameIt.Application.Features.Category.Commands.CreateCategory;
using GameIt.Application.Features.Category.Commands.DeleteCategory;
using GameIt.Application.Features.Category.Commands.UpdateCategory;
using GameIt.Application.Features.Category.Queries.GetAllCategoryLists;
using GameIt.Application.Features.Category.Queries.GetCategoryByName;
using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CategoryIt.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get: api/Category
    [HttpGet]
    public async Task<IActionResult> GetCategorys()
    {
        var query = new GetAllCategoriesListQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Get: api/Category/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
    {
        var query = new GetCategoryDetailsQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Get: api/Category/name/{name}
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetCategoryByName([FromRoute] string name)
    {
        var query = new GetCategoryByNameQuery(name);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // Post: api/Category
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result }, result);
    }

    // Delete: api/Category/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id};
        await _mediator.Send(command);
        return NoContent();
    }

    // Put: api/Category/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Category ID mismatch.");
        }
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
