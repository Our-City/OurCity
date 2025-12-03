using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Comments;

public class CommentResponseDto
{
    public required Guid Id { get; set; }
    public required Guid PostId { get; set; }
    public required Guid AuthorId { get; set; }
    public required string Content { get; set; }
    public string? AuthorName { get; set; }
    public required int UpvoteCount { get; set; }
    public required int DownvoteCount { get; set; }
    public required VoteType VoteStatus { get; set; }
    public required bool CanMutate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
