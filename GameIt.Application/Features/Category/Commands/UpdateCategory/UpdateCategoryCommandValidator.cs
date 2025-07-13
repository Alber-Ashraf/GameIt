using FluentValidation;
using GameIt.Application.Interfaces.Persistence;

namespace GameIt.Application.Features.Category.Commands.UpdateCategory;
    
public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required    .");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x)
            .MustAsync(BeUniqueName)
            .WithMessage("A Category with the same name already exists.");
    }

    private async Task<bool> BeUniqueName(UpdateCategoryCommand command, CancellationToken token)
    {
        return await _unitOfWork.Categories.IsCategoryNameUniqueForUpdate(command.Id, command.Name, token);
    }
}
