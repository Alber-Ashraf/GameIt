using AutoMapper;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;

public class GetUserWishlistListQueryHandler : IRequestHandler<GetUserWishlistListQuery, List<WishlistListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserWishlistListQueryHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<WishlistListDto>> Handle(
        GetUserWishlistListQuery request,
        CancellationToken token)
    {
        // Fetch Wishlists with related data
        var Wishlists = await _unitOfWork.Wishlists
            .GetUserWishlistWithDetailsAsync(request.UserId, token);
        
        if (!Wishlists.Any())
            return new List<WishlistListDto>();

        return _mapper.Map<List<WishlistListDto>>(Wishlists);
    }
}
