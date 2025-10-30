namespace OurCity.Api.Infrastructure.Database;

public class Tag
{
    public int Id { get; set; }

    public required string Name { get; set; }

     public List<Post>? Posts { get; set; }
}