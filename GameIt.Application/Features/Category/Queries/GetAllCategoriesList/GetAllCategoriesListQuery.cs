using GameIt.Application.Features.Category.Queries.GetAllGameLists;
using MediatR;

namespace CategoryIt.Application.Features.Category.Queries.GetAllCategoryLists;

public class GetAllCategoriesListQuery : IRequest<List<CategoriesListDto>> {}
