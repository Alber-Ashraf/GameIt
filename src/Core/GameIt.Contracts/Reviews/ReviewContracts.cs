using System.ComponentModel.DataAnnotations;

namespace GameIt.Contracts.Reviews;

public class ReviewItemDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class UpsertReviewRequestDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string? UserId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
    public int Rating { get; set; } = 5;

    [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
    public string Comment { get; set; } = string.Empty;
}

public class CreateReviewResponseDto
{
    public Guid ReviewId { get; set; }
}
