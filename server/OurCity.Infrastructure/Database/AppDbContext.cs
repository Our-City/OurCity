using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurCity.Infrastructure.Database.Models;

namespace OurCity.Infrastructure.Database;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }
}
