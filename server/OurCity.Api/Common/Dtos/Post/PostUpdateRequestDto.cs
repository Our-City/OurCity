using System.ComponentModel.DataAnnotations;
using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Post;

public class PostUpdateRequestDto
{
    [StringLength(
        50,
        MinimumLength = 1,
        ErrorMessage = "Title must be between 1 and 50 characters"
    )]
    [RegularExpression(@"^\s*\S.*$", ErrorMessage = "Title cannot be only whitespace")]
    public string? Title { get; set; }

    [StringLength(
        500,
        MinimumLength = 1,
        ErrorMessage = "Description must be between 1 and 500 characters"
    )]
    [RegularExpression(@"^\s*\S[\s\S]*$", ErrorMessage = "Description cannot be only whitespace")]
    public string? Description { get; set; }

    [StringLength(50, ErrorMessage = "Location cannot exceed 50 characters")]
    public string? Location { get; set; }

    public List<Guid>? TagIds { get; set; }
}
