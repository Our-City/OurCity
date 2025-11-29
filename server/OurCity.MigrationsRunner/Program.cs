using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Infrastructure.Database.App;
using OurCity.Api.Infrastructure.Database.Host;
using OurCity.Api.Infrastructure.Database.Utils;

/*
 * Code taken largely from ChatGPT, asking how to run migrations for a docker postgres container
 * This should work for any DB through connection string, but thats where it came from
 */

try
{
    //PRECONDITION: HOST CONNECTION STRING MUST BE PASSED IN
    string? hostConnectionString = Environment.GetEnvironmentVariable("HOST_CONNECTION_STRING");
    if (hostConnectionString is null)
        throw new InvalidOperationException("HOST_CONNECTION_STRING required as environment variable");

    //Run migrations for Host DB
    Console.WriteLine("Migrating Host DB...");
    
    var options = new DbContextOptionsBuilder<HostDbContext>().UseNpgsql(hostConnectionString).Options;
    using var hostDbContext = new HostDbContext(options);

    hostDbContext.Database.Migrate();

    Console.WriteLine("Host DB migration complete!");

    //Run migrations for each TenantDB 
    Console.WriteLine("Migrating Tenant DBs...");
    
    var tenants = hostDbContext.Tenants.ToList();
    foreach (var tenant in tenants)
    {
        var tenantConnectionStringFactory = new TenantConnectionStringFactory(hostDbContext);
        var tenantProvider = new TenantProvider { TenantName = tenant.Name };

        using var tenantDbContext = new AppDbContext(
            new DbContextOptions<AppDbContext>(), 
            tenantProvider, 
            tenantConnectionStringFactory
        );

        tenantDbContext.Database.Migrate();
    }

    Console.WriteLine("All tenant DB migrations complete!");

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed: {ex.Message}");
    return 1;
}
