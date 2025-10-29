namespace OurCity.Api.Common.Dtos.Comments;

public class CommentResponseDto
{
    public required int Id { get; set; }
    public required Guid PostId { get; set; }
    public required Guid AuthorId { get; set; }
    public required string Content { get; set; }
    public required int Votes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
