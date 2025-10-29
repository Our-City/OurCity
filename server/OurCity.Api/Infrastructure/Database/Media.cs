namespace OurCity.Api.Infrastructure.Database;

public class Media
{
    public int Id { get; set; }
    public int PostId { get; set; }                     //foreign key property
    public required string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid PostId { get; set; }
    public Post? Post { get; set; }
}
