using MediatR;
namespace GameIt.Application.Features.Category.Queries.GetCategoryDetails;

public record GetCategoryDetailsQuery(Guid Id) : IRequest<CategoryDetailsDto>;
