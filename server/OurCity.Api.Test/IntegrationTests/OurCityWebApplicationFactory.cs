using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OurCity.Api.Extensions;
using OurCity.Api.Infrastructure.Database;
using Testcontainers.PostgreSql;

namespace OurCity.Api.Test.IntegrationTests;

/// <summary>
/// Setup a WebApplicationFactory for the OurCity app, using Docker for Postgres DB
/// </summary>
/// <credits>
/// Code modified from ChatGPT response just asking how to call API endpoints for testing
///
/// Claude assisted implementing SeedTestDataAsync()
/// Promt: Following the common practices for ASP.NET, provide me with a sample boilerplate for a
/// method to seed test data using the passed in data
/// </credits>
public class OurCityWebApplicationFactory : WebApplicationFactory<Program>
{
    public readonly Guid StubUserId = Guid.NewGuid();
    public readonly string StubUsername = "StubUser";
    public readonly string StubPassword = "TestPassword1!";

    public readonly Guid StubPostId = Guid.NewGuid();
    public readonly string StubPostTitle = "Test Title";
    public readonly string StubPostDescription = "Test Description";

    public readonly Guid StubPostWithCommentId = Guid.NewGuid();
    public readonly Guid StubCommentId = Guid.NewGuid();
    public readonly string StubCommentContent = "Test Content";

    private readonly PostgreSqlContainer _postgres;

    public OurCityWebApplicationFactory()
    {
        _postgres = new PostgreSqlBuilder().WithImage("postgres:16.10").WithCleanUp(true).Build();
    }

    public async Task StartDbAsync()
    {
        await _postgres.StartAsync();
    }

    public async Task StopDbAsync()
    {
        await _postgres.StopAsync();
        await _postgres.DisposeAsync();
    }

    public async Task SeedTestDataAsync(Func<AppDbContext, UserManager<User>, Task> seedAction)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await seedAction(db, userManager);
        await db.SaveChangesAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"PostVotes\", \"CommentVotes\", \"Comments\", \"Posts\", \"Tags\", \"Media\" RESTART IDENTITY CASCADE;"
        );
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestEnvironment();

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

            //stub post
            var stubPost = new Post
            {
                Id = StubPostId,
                AuthorId = StubUserId,
                Title = StubPostTitle,
                Description = StubPostDescription,
            };
            db.Posts.Add(stubPost);

            //stub comment
            var stubPostWithComment = new Post
            {
                Id = StubPostWithCommentId,
                AuthorId = StubUserId,
                Title = "Title",
                Description = "Description",
            };
            var stubComment = new Comment
            {
                Id = StubCommentId,
                PostId = StubPostWithCommentId,
                AuthorId = StubUserId,
                Content = StubCommentContent,
                IsDeleted = false,
                Votes = [],
            };
            db.Posts.Add(stubPostWithComment);
            db.Comments.Add(stubComment);

            db.SaveChangesAsync().Wait();
        });
    }
}
