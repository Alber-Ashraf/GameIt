using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Category.Queries.GetCategoryByName;

public class GetCategoryByNameQueryHandler : IRequestHandler<GetCategoryByNameQuery, CategoryDetailsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetCategoryByNameQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<CategoryDetailsDto> Handle(GetCategoryByNameQuery request, CancellationToken token)
    {
        // Query the database for category details
        var existingCategory = await _unitOfWork.Categories.GetByNameAsync(request.Name, token);

        // Validate if the Category exists
            if (existingCategory == null)
                throw new NotFoundException(request.Name);

        // Convert the Category entities to CategoryDetailsDto using AutoMapper
        var data = _mapper.Map<CategoryDetailsDto>(existingCategory);

        // Return the list of CategoryDetailsDto
        return data;
    }
}
