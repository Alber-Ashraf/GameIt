using GameIt.Application.Features.Category.Queries.GetCategoryDetails;
using MediatR;

namespace CategoryIt.Application.Features.Category.Queries.GetCategoryDetails;

public record GetCategoryDetailsQuery(Guid Id) : IRequest<CategoryDetailsDto>;
