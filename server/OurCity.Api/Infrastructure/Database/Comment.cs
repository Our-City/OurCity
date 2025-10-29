namespace OurCity.Api.Infrastructure.Database;

public class Comment
{
    public int Id { get; set; }

    public Post Post { get; set; } = null!;
    public required Guid PostId { get; set; }

    public User Author { get; set; } = null!;
    public required Guid AuthorId { get; set; }

    public required string Content { get; set; }

    public List<Guid> UpvotedUserIds { get; set; } = new();

    public List<Guid> DownvotedUserIds { get; set; } = new();

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
