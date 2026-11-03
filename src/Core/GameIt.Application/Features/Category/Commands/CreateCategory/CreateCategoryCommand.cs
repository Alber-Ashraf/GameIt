using MediatR;

namespace GameIt.Application.Features.Category.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string? CategoryName
    {
        get => Name;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                Name = value;
        }
    }
}
