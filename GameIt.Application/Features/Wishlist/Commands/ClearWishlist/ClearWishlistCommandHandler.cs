using FluentValidation;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Wishlist.Commands.ClearWishlist;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class ClearWishlistCommandHandler : IRequestHandler<ClearWishlistCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public ClearWishlistCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ClearWishlistCommand request, CancellationToken token)
    {
        // Validate the request
        var validator = new ClearWishlistCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Clear Wishlist Request", validationResult);

        // Check if the user has a wishlist
        var wishlist = await _unitOfWork.Wishlists.AnyWishlistItemsAsync(request.UserId, token);

        if (!wishlist)
            throw new NotFoundException("Wishlist not found for the user.");

        // Clear the wishlist
        await _unitOfWork.Wishlists.CleanWishlistAsync(request.UserId, token);
        await _unitOfWork.SaveChangesAsync(token);

        return Unit.Value;
    }
}