namespace OurCity.Api.Infrastructure.Database;

public class PostBookmark
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public DateTime BookmarkedAt { get; set; }

    // Navigation Properties
    public User? User { get; set; }

    public Post? Post { get; set; }
}