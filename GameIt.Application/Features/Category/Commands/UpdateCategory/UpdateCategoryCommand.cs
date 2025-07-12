using MediatR;

namespace GameIt.Application.Features.Category.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
