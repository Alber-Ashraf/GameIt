using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Game.Commands.DeleteGame;

public class DeleteGameCommandValidator : AbstractValidator<DeleteGameCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGameCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Game ID is required.");
    }
}
