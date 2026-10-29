using MediatR;

namespace GameIt.Application.Features.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommand : IRequest<Guid>
{
    public Guid GameId { get; set; }
}