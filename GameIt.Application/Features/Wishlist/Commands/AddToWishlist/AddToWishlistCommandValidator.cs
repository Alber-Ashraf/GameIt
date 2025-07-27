using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommandValidator : AbstractValidator<AddToWishlistCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddToWishlistCommandValidator(IUnitOfWork unitOfWork)
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
