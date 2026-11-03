using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using GameIt.Contracts.Games;

namespace GameIt.Contracts.Categories;

public class CategorySummaryDto
{
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name
    {
        get => CategoryName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                CategoryName = value;
        }
    }
}

public class CategoryDetailDto
{
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name
    {
        get => CategoryName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                CategoryName = value;
        }
    }

    public List<GameSummaryDto>? Games { get; set; }
}

public class UpsertCategoryRequestDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, MinimumLength = 2)]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name
    {
        get => CategoryName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                CategoryName = value;
        }
    }
}
