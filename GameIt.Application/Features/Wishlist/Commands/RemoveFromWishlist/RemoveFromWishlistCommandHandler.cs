using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public RemoveFromWishlistCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(RemoveFromWishlistCommand request, CancellationToken token)
    {
        // Validate the request
        var validator = new RemoveFromWishlistCommandValidator(_unitOfWork);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Wishlist Item", validationResult);

        // Check if the game is in the user's wishlist
        var exists = await _unitOfWork.Wishlists.IsGameInWishlistAsync(request.UserId, request.GameId);

        if (!exists)
            throw new BadRequestException("This game is not in your wishlist.");

        // Remove the item from the wishlist
        await _unitOfWork.Wishlists.RemoveFromWishlistAsync(request.UserId, request.GameId, token);

        // Save changes
        await _unitOfWork.SaveChangesAsync(token);

        // Return Unit to indicate success
        return Unit.Value;

    }
}