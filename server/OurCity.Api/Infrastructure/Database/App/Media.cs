namespace OurCity.Api.Infrastructure.Database.App;

public class Media
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public Post? Post { get; set; }
}
