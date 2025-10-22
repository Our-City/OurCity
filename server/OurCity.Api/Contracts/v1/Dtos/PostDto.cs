namespace OurCity.Api.Contracts.v1.Dtos;

public class PostDto
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int Votes { get; set; }
    public required string? Location { get; set; }
    public required List<string> Images { get; set; } = new();
    public List<int> CommentIds { get; set; } = new();
}