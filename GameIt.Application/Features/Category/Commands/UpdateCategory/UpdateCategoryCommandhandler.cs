using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Category.Commands.UpdateCategory;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace CategoryIt.Application.Features.Category.Commands.UpdateCategory;

public class UpdateCategoryCommandhandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public UpdateCategoryCommandhandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken token)
    {
        // Validate
        var validator = new UpdateCategoryCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Category", validationResult);

        // Fetch existing category
        var existingCategory = await _unitOfWork.Categories.GetByIdAsync(request.Id);
        if (existingCategory == null)
            throw new NotFoundException(nameof(Category), request.Id);

        // Update only if Name is provided and different
        if (request.Name != null && existingCategory.Name != request.Name)
        {
            existingCategory.Name = request.Name;
            _unitOfWork.Categories.Update(existingCategory);
            await _unitOfWork.SaveChangesAsync(token);
        }

        return Unit.Value;
    }
}
