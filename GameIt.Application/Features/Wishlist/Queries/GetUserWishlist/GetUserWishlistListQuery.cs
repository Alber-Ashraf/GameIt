using MediatR;

namespace GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;

public record GetUserWishlistListQuery() : IRequest<List<WishlistListDto>> {}