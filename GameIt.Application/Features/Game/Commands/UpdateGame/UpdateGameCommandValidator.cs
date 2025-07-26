using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Game.Commands.UpdateGame;

public class UpdateGameCommandValidator : AbstractValidator<UpdateGameCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGameCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Game ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Game name is required.")
            .MaximumLength(100).WithMessage("Game name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Game description is required.")
            .MaximumLength(500).WithMessage("Game description must not exceed 500 characters.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");

        RuleFor(x => x.FileSizeInBytes)
            .NotEmpty().WithMessage("Game size is required.");

        RuleFor(x => x.DownloadLink)
            .NotEmpty().WithMessage("Download link is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category id is required.");

        RuleFor(x => x)
            .MustAsync(BeUniqueName)
            .WithMessage("A game with the same name already exists.");
    }

    private async Task<bool> BeUniqueName(UpdateGameCommand command, CancellationToken token)
    {
        return await _unitOfWork.Games.IsGameNameUniqueForUpdate(command.Id, command.Name);
    }
}
