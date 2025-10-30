using OurCity.Api.Common.Dtos.Tag;
using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Post;

public class PostResponseDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? Location { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public int CommentCount { get; set; }
    public PostVisibility Visibility { get; set; }
    public required List<TagResponseDto> Tags { get; set; }
    public VoteType VoteStatus { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
