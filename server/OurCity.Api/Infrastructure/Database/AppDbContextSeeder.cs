using Microsoft.AspNetCore.Identity;
using OurCity.Api.Common;

namespace OurCity.Api.Infrastructure.Database;

/// <credits>
/// Asked ChatGPT for best way to seed identity users
/// </credits>
public class AppDbContextSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        //PRECONDITION: Admin Password required in configuration
        var config = services.GetRequiredService<IConfiguration>();
        if (config["AdminPassword"] is null)
            throw new InvalidOperationException("Admin Password is missing in configuration.");

        //Required services for seeding
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<UserRole>>();

        //Seed roles
        var roles = new[] { UserRoles.User, UserRoles.Admin };

        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new UserRole(role));

        //Seed admin user
        var adminUsername = "admin";
        var adminPassword = config["AdminPassword"]!;

        var adminUser = await userManager.FindByNameAsync(adminUsername);
        if (adminUser is null)
        {
            adminUser = new User { UserName = adminUsername };

            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }
    }
}
