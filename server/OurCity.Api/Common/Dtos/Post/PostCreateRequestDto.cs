using System.ComponentModel.DataAnnotations;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Common.Dtos.Post;

public class PostCreateRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
    public required string Title { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Required(ErrorMessage = "Description is required")]
    public required string Description { get; set; }

    [StringLength(50, ErrorMessage = "Location cannot exceed 50 characters")]
    public string? Location { get; set; }

    public List<Tag>? Tags { get; set; } = new(); 
}
