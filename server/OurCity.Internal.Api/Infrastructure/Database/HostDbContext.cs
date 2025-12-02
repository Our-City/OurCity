using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OurCity.Internal.Api.Infrastructure.Database;

public class HostDbContext : IdentityDbContext<HostAdmin, IdentityRole<Guid>, Guid>
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<HostAdmin> HostAdmins { get; set; }

    public HostDbContext(DbContextOptions<HostDbContext> options)
        : base(options) { }
}
