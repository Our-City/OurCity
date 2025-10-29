using OurCity.Api.Common.Dtos.Image;

namespace OurCity.Api.Common.Dtos.Post;

public class PostResponseDto
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int Votes { get; set; }
    public required string? Location { get; set; }
    public required List<ImageDto> Images { get; set; } = new();
    public List<int> CommentIds { get; set; } = new();
}
