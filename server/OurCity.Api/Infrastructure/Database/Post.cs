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

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public User? Author { get; set; }

    public List<Comment> Comments { get; set; } = new(); 

    public List<Tag> Tags { get; set; } = new();

    public List<PostVote> Votes { get; set; } = new();
}
