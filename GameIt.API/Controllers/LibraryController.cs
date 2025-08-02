using GameIt.Application.Features.Library.Queries.GetUserLibrary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LibraryController : Controller
{
    private readonly IMediator _mediator;
    public LibraryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    // Get: api/library/users/{userId}
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(List<LibraryListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<LibraryListDto>>> GetUserLibrary(
        CancellationToken token = default)
    {
        var query = new GetUserLibraryListQuery();
        var library = await _mediator.Send(query, token);
        return Ok(library);
    }
}
