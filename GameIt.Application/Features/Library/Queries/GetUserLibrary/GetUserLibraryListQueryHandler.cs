using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Library.Queries.GetUserLibrary;

public class GetUserLibraryListQueryHandler : IRequestHandler<GetUserLibraryListQuery, List<LibraryListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;


    public GetUserLibraryListQueryHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<LibraryListDto>> Handle(
        GetUserLibraryListQuery request,
        CancellationToken cancellationToken)
    {

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Fetch Librarys with related data
        var Librarys = await _unitOfWork.Libraries
            .GetLibraryForUserAsync(userId, cancellationToken);
        
        if (!Librarys.Any())
            return new List<LibraryListDto>();

        return _mapper.Map<List<LibraryListDto>>(Librarys);
    }
}
