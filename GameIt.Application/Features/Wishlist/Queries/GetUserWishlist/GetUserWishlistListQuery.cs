using MediatR;

namespace GameIt.Application.Features.Wishlist.Queries.GetUserWishlist;

public record GetUserWishlistListQuery(string UserId) : IRequest<List<WishlistListDto>> {}