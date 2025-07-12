using GameIt.Application.Features.Game.Queries.GetAllGameLists;

namespace GameIt.Application.Features.Category.Queries.GetAllGameLists;

public class CategoriesListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}