namespace OurCity.Api.Infrastructure.Database;

public class Post
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public User Author { get; set; } = null!;
    public Guid AuthorId { get; set; }

    public string? Location { get; set; }

    public List<Guid> UpvotedUserIds { get; set; } = new();
    public List<Guid> DownvotedUserIds { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Image> Images { get; set; } = new();

    public List<Comment> Comments { get; set; } = new();
}
