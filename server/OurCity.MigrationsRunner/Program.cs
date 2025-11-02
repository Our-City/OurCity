using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

/*
 * Code taken largely from ChatGPT, asking how to run migrations for a docker postgres container
 * This should work for any DB through connection string, but thats where it came from
 */

try
{
    Console.WriteLine("Migrating...");

    //configure dbcontext
    string? connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    if (connectionString is null)
        throw new Exception("Connection string required as environment variable");

    var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString).Options;

    //run migrations
    using var dbContext = new AppDbContext(options);
    dbContext.Database.Migrate();

    Console.WriteLine("Migration complete");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Migration failed: {ex.Message}");
    return 1;
}
