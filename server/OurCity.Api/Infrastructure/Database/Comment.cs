namespace OurCity.Api.Infrastructure.Database;

public class Comment
{
    public Guid Id { get; set; }

    public required Guid PostId { get; set; }

    public required Guid AuthorId { get; set; }

    public required string Content { get; set; }

    public required bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public Post? Post { get; set; }

    public User? Author { get; set; }

    public required List<CommentVote> Votes { get; set; } = new();
}
