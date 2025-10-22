namespace OurCity.Api.Contracts.v1.Dtos;

public class PostCommentDto
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public int? AuthorId { get; set; }
    public string? Content { get; set; }
    public required int Votes { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}