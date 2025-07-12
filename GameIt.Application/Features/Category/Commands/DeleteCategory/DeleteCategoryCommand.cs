using MediatR;

namespace CategoryIt.Application.Features.Category.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
