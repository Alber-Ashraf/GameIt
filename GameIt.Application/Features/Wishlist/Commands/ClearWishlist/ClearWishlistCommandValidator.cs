using FluentValidation;
using GameIt.Application.Features.Wishlist.Commands.ClearWishlist;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class ClearWishlistCommandValidator : AbstractValidator<ClearWishlistCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ClearWishlistCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
}