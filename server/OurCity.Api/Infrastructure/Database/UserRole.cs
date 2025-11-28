using Microsoft.AspNetCore.Identity;

namespace OurCity.Api.Infrastructure.Database;

public class UserRole : IdentityRole<Guid>
{
    //Constructors required for EFCore
    public UserRole() { }

    public UserRole(string roleName)
        : base(roleName) { }
}
