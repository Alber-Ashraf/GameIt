using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandValidator : AbstractValidator<RemoveFromWishlistCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromWishlistCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("GameId is required.")
            .MustAsync(GameExists).WithMessage("Game does not exist.");

    }
    private async Task<bool> GameExists(Guid gameId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Games.ExistsAsync(gameId, cancellationToken);
    }
}