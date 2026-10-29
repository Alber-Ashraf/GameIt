using System.ComponentModel.DataAnnotations;
using GameIt.Contracts.Games;

namespace GameIt.Contracts.Categories;

public class CategorySummaryDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class CategoryDetailDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<GameSummaryDto>? Games { get; set; }
}

public class UpsertCategoryRequestDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string CategoryName { get; set; } = string.Empty;
}
