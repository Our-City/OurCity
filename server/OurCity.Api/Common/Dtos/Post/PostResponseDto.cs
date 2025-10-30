using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Common.Dtos.Post;

public class PostResponseDto
{
    public required Guid Id { get; set; }
    public required Guid AuthorId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string? Location { get; set; }
    public required int UpvoteCount { get; set; }
    public required int DownvoteCount { get; set; }
    public required int CommentCount { get; set; }
    public required PostVisibility Visibility { get; set; }
    public required List<Tag> Tags { get; set; }
    public required VoteType VoteStatus { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
