namespace OurCity.Api.Common.Dtos.Comments;

public class CommentUpvoteResponseDto
{
    public required int Id { get; set; }
    public required Guid PostId { get; set; }
    public required Guid AuthorId { get; set; }
    public required bool Upvoted { get; set; }
}
