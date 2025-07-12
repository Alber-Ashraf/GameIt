using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new DeleteCategoryCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Category", validationResult);

        // Fetch existing category
        var existingCategory = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (existingCategory == null)
            throw new NotFoundException(nameof(Category), request.Id);

        // Delete the Category entity from the repository
        _unitOfWork.Categories.Delete(existingCategory);

        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();

        // Return Unit.Value to indicate successful completion
        return Unit.Value;
    }
}
