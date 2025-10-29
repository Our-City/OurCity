using System.ComponentModel.DataAnnotations;

namespace OurCity.Api.Common.Dtos.Comments;

public class CommentCreateRequestDto
{
    [Required(ErrorMessage = "AuthorId is required")]
    public required Guid AuthorId { get; set; }

    [Required(ErrorMessage = "Content is required")]
    public required string Content { get; set; }
}
