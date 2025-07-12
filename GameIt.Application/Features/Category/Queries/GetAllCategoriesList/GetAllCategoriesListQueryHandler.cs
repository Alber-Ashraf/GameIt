using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Features.Category.Queries.GetAllGameLists;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Category.Queries.GetAllCategoryLists;

public class GetAllCategoriesListQueryHandler : IRequestHandler<GetAllCategoriesListQuery, List<CategoriesListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public GetAllCategoriesListQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CategoriesListDto>> Handle(GetAllCategoriesListQuery request, CancellationToken cancellationToken)
    {
        // Query the database for all Categorys Lists
        var categories = await _unitOfWork.Categories.GetAllAsync();

        // Validate if any Categorys exist
        if (!categories.Any())
            throw new NotFoundException(nameof(Category), "No Categories found");

        // Convert the Category entities to CategoryDetailsDto using AutoMapper
        var data = _mapper.Map<List<CategoriesListDto>>(categories);

        // Return the list of CategoryDetailsDto
        return data;
    }
}
