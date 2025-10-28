namespace OurCity.Api.Infrastructure.Database;

public class Tags
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Post> Posts { get; set; } = new(); // Navigation property to related Posts
}
