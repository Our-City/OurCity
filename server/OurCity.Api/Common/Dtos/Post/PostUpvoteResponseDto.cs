namespace OurCity.Api.Common.Dtos;

public class PostUpvoteResponseDto
{
    public required Guid Id { get; set; }
    public required Guid AuthorId { get; set; }
    public required bool Upvoted { get; set; }
}
