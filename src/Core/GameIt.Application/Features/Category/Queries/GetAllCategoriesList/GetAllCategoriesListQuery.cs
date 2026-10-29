using GameIt.Application.Features.Category.Queries.GetAllGameLists;
using MediatR;

namespace GameIt.Application.Features.Category.Queries.GetAllCategoryLists;

public class GetAllCategoriesListQuery : IRequest<List<CategoriesListDto>> {}
