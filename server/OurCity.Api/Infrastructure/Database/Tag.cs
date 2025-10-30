namespace OurCity.Api.Infrastructure.Database;

public class Tag
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    // Navigation Properties
    public List<Post> Posts { get; set; } = new();
}
