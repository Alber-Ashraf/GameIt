using FluentValidation;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Wishlist.Commands.ClearWishlist;
using GameIt.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class ClearWishlistCommandHandler : IRequestHandler<ClearWishlistCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _contextAccessor;
    public ClearWishlistCommandHandler(
        IUnitOfWork unitOfWork,
        IHttpContextAccessor contextAccessor)
    {
        _unitOfWork = unitOfWork;
        _contextAccessor = contextAccessor;
    }

    public async Task<Unit> Handle(ClearWishlistCommand request, CancellationToken token)
    {
        // Validate the request
        var validator = new ClearWishlistCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Clear Wishlist Request", validationResult);

        // Get the User ID from the HTTP context
        string? userId = _contextAccessor.HttpContext?.User?.FindFirstValue("uid");

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User must be authenticated");

        // Check if the user has a wishlist
        var wishlist = await _unitOfWork.Wishlists.AnyWishlistItemsAsync(userId, token);

        if (!wishlist)
            throw new NotFoundException("Wishlist not found for the user.");

        // Clear the wishlist
        await _unitOfWork.Wishlists.CleanWishlistAsync(userId, token);
        await _unitOfWork.SaveChangesAsync(token);

        return Unit.Value;
    }
}