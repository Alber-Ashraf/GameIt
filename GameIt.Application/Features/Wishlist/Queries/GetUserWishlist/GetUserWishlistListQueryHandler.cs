using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;

public class GetUserWishlistListQueryHandler : IRequestHandler<GetUserWishlistListQuery, List<WishlistListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetUserWishlistListQueryHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<WishlistListDto>> Handle(
        GetUserWishlistListQuery request,
        CancellationToken token)
    {
        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Fetch Wishlists with related data
        var Wishlists = await _unitOfWork.Wishlists
            .GetUserWishlistWithDetailsAsync(userId, token);
        
        if (!Wishlists.Any())
            return new List<WishlistListDto>();

        return _mapper.Map<List<WishlistListDto>>(Wishlists);
    }
}
