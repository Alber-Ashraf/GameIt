using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateCategoryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x)
            .MustAsync(BeUniqueName)
            .WithMessage("A Category with the same name already exists.");
    }

    private async Task<bool> BeUniqueName(CreateCategoryCommand command, CancellationToken token)
    {
        return await _unitOfWork.Categories.IsCategoryUniqueForCreate(command.Name, token); 
    }
}
