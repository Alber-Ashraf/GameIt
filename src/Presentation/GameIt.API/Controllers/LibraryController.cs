using GameIt.Application.Features.Library.Queries.GetUserLibrary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameIt.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LibraryController : ControllerBase
{
    private readonly IMediator _mediator;

    public LibraryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/library
    [HttpGet]
    [ProducesResponseType(typeof(List<LibraryListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LibraryListDto>>> GetUserLibraryDirect(
        CancellationToken token = default)
    {
        var query = new GetUserLibraryListQuery();
        var library = await _mediator.Send(query, token);
        return Ok(library);
    }

    // GET: api/library/users/{userId}
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(List<LibraryListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LibraryListDto>>> GetUserLibrary(
        [FromRoute] string userId,
        CancellationToken token = default)
    {
        var query = new GetUserLibraryListQuery();
        var library = await _mediator.Send(query, token);
        return Ok(library);
    }
}
