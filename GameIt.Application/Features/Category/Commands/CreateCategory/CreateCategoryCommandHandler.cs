using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validate the request 
        var validator = new CreateCategoryCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Category", validationResult);

        // Map the request to a Category entity
        var categoryToCreate = _mapper.Map<Domain.Category>(request);

        // Add to the CORRECT repository (Categories, not Games)
        await _unitOfWork.Categories.CreateAsync(categoryToCreate);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return the new category's ID
        return categoryToCreate.Id;
    }
}
