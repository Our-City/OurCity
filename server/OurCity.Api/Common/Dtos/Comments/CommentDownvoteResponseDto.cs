namespace OurCity.Api.Common.Dtos.Comments;

public class CommentDownvoteResponseDto
{
    public required int Id { get; set; }
    public required Guid PostId { get; set; }
    public required Guid AuthorId { get; set; }
    public required bool Downvoted { get; set; }
}
