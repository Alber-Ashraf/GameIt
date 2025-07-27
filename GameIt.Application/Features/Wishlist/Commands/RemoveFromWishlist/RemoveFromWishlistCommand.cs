using MediatR;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommand : IRequest<Unit>
{
    public Guid GameId { get; set; }
}