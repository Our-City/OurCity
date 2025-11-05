using System.ComponentModel.DataAnnotations;

namespace OurCity.Api.Common.Dtos.Post;

public class PostCreateRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(
        50,
        MinimumLength = 1,
        ErrorMessage = "Title must be between 1 and 50 characters"
    )]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Title cannot be only whitespace")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(
        500,
        MinimumLength = 1,
        ErrorMessage = "Description must be between 1 and 500 characters"
    )]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Description cannot be only whitespace")]
    public required string Description { get; set; }

    [StringLength(50, ErrorMessage = "Location cannot exceed 50 characters")]
    public string? Location { get; set; }

    public List<Guid> TagIds { get; set; } = new();
}
