using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;
    public RemoveFromWishlistCommandHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }
    public async Task<Unit> Handle(RemoveFromWishlistCommand request, CancellationToken token)
    {
        // Validate the request
        var validator = new RemoveFromWishlistCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Wishlist Item", validationResult);

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Check if the game is in the user's wishlist
        var exists = await _unitOfWork.Wishlists.IsGameInWishlistAsync(userId, request.GameId);

        if (!exists)
            throw new BadRequestException("This game is not in your wishlist.");

        // Remove the item from the wishlist
        await _unitOfWork.Wishlists.RemoveFromWishlistAsync(userId, request.GameId, token);
        await _unitOfWork.SaveChangesAsync(token);

        // Return Unit to indicate success
        return Unit.Value;

    }
}