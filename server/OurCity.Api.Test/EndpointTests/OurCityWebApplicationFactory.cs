using Microsoft.AspNetCore.Hosting;
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
public class OurCityWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;

    public OurCityWebApplicationFactory()
    {
        _postgres = new PostgreSqlBuilder().WithImage("postgres:16.10").Build();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        ;
    }

    public new async Task DisposeAsync()
    {
        await _postgres.StopAsync();
    }

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
        });
    }
}
