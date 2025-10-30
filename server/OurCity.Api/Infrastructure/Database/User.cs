using Microsoft.AspNetCore.Identity;

namespace OurCity.Api.Infrastructure.Database;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsAdmin { get; set; } = false;

    public bool IsBanned { get; set; } = false;

    public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public List<Post> Posts { get; set; } = new();

    public List<Comment> Comments { get; set; } = new();

    public List<PostVote> PostVotes { get; set; } = new();
}
