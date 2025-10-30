using OurCity.Api.Common.Enum;

namespace OurCity.Api.Infrastructure.Database;

public class Post
{
    public Guid Id { get; set; }
    
    public Guid AuthorId { get; set; }  

    public required string Title { get; set; }

    public required string Description { get; set; }

    public string? Location { get; set; }

    public PostVisibility Visisbility { get; set; } = PostVisibility.Published;

    public List<Tag> Tags { get; set; } = new();

    public List<PostVote> Votes { get; set; } = new();

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation Property
    public User? Author { get; set; }
    
    public List<Comment>? Comments { get; set; }
}
