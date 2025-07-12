using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace CategoryIt.Application.Features.Category.Queries.GetCategoryDetails;

public class GetCategoryDetailsQueryHandler : IRequestHandler<GetCategoryDetailsQuery, CategoryDetailsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetCategoryDetailsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<CategoryDetailsDto> Handle(GetCategoryDetailsQuery request, CancellationToken cancellationToken)
    {
        // Query the database for all Categorys Details
        var existingCategory = await _unitOfWork.Categories.GetByIdAsync(request.Id);

        // Validate if the Category exists
        if (existingCategory == null)
            throw new NotFoundException(nameof(Category), request.Id);

        // Convert the Category entities to CategoryDetailsDto using AutoMapper
        var data = _mapper.Map<CategoryDetailsDto>(existingCategory);

        // Return the list of CategoryDetailsDto
        return data;
    }
}
