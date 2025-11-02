using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OurCity.Api.Infrastructure.Database;
using Testcontainers.PostgreSql;

namespace OurCity.Api.Test.EndpointTests;

/// <summary>
/// Setup a WebApplicationFactory for the OurCity app, using Docker for Postgres DB
/// </summary>
/// <credits>
/// Code modified from ChatGPT response just asking how to call API endpoints for testing
/// </credits>
public class OurCityWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly Guid StubUserId = Guid.NewGuid();
    public readonly string StubUsername = "StubUser";
    public readonly string StubPassword = "TestPassword1!";

    private readonly PostgreSqlContainer _postgres;

    public OurCityWebApplicationFactory()
    {
        _postgres = new PostgreSqlBuilder().WithImage("postgres:16.10").WithCleanUp(true).Build();
    }

    public async Task StartDbAsync() => await _postgres.StartAsync();

    public async Task StopDbAsync() => await _postgres.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContextPool<AppDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString())
            );

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();

            //stub user
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var newUser = new User { Id = StubUserId, UserName = StubUsername };
            userManager.CreateAsync(newUser, StubPassword).Wait();
        });
    }
}
