using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using MediatR;

namespace GameIt.Application.Features.Category.Queries.GetCategoryByName;

public record GetCategoryByNameQuery(string Name) 
    : IRequest<CategoryDetailsDto>;
