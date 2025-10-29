namespace OurCity.Api.Common.Dtos;

public class PostDownvoteResponseDto
{
    public required Guid Id { get; set; }
    public required Guid AuthorId { get; set; }
    public required bool Downvoted { get; set; }
}
