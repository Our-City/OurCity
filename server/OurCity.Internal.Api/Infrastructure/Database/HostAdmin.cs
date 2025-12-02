using Microsoft.AspNetCore.Identity;

namespace OurCity.Internal.Api.Infrastructure.Database;

public class HostAdmin : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
