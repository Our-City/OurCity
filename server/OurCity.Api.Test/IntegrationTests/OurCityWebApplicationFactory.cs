using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OurCity.Api.Common;
using OurCity.Api.Extensions;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Infrastructure.Database.App;
using OurCity.Api.Infrastructure.Database.Host;
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

    public readonly Guid StubUserId2 = Guid.NewGuid();
    public readonly string StubUsername2 = "StubUser2";
    public readonly string StubPassword2 = "TestPassword2!";

    public readonly Guid AdminUserId = Guid.NewGuid();
    public readonly string AdminUsername = "AdminUser";
    public readonly string AdminPassword = "AdminPassword1!";

    public readonly Guid HighlyReportedUserId = Guid.NewGuid();
    public readonly string HighlyReportedUsername = "HighlyReportedUser";
    public readonly string HighlyReportedPassword = "HighlyReportedPassword1!";

    public readonly Guid StubPostId = Guid.NewGuid();
    public readonly string StubPostTitle = "Test Title";
    public readonly string StubPostDescription = "Test Description";

    public readonly Guid StubPostWithCommentId = Guid.NewGuid();
    public readonly Guid StubCommentId = Guid.NewGuid();
    public readonly string StubCommentContent = "Test Content";

    private readonly PostgreSqlContainer _hostPostgres;
    private readonly PostgreSqlContainer _tenantPostgres;

    public OurCityWebApplicationFactory()
    {
        _hostPostgres = new PostgreSqlBuilder().WithImage("postgres:16.10").WithCleanUp(true).Build();
        _tenantPostgres = new PostgreSqlBuilder().WithImage("postgres:16.10").WithCleanUp(true).Build();
    }

    public async Task StartDbAsync()
    {
        await _hostPostgres.StartAsync();
        await _tenantPostgres.StartAsync();
    }

    public async Task StopDbAsync()
    {
        await _hostPostgres.StopAsync();
        await _hostPostgres.DisposeAsync();
        await _tenantPostgres.StopAsync();
        await _tenantPostgres.DisposeAsync();
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
            "TRUNCATE TABLE \"PostVotes\", \"CommentVotes\", \"Comments\", \"Posts\", \"Tags\", \"Media\", \"UserReports\" RESTART IDENTITY CASCADE;"
        );
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestEnvironment();

        builder.ConfigureServices(services =>
        {
            //Remove HostDbContext and AppDbContext registrations from Program.cs
            var hostDbService = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<HostDbContext>)
            );
            if (hostDbService != null)
                services.Remove(hostDbService);

            var appDbService = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );
            if (appDbService != null)
                services.Remove(appDbService);

            //Add new HostDbContext and AppDbContext registrations
            services.AddDbContextPool<HostDbContext>(options =>
                options.UseNpgsql(_hostPostgres.GetConnectionString())
            );
            services.AddDbContext<AppDbContext>();

            //Run migrations for new HostDbContext and AppDbContext
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var hostDb = scope.ServiceProvider.GetRequiredService<HostDbContext>();
            var tenantDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            hostDb.Database.Migrate();

            var tenant = new Tenant
            {
                DbName = "TestTenant",
                DbUser = "TestTenantDbUser",
                DbPassword = "TestTenantDbPassword",
                DbServer = "localhost"
            };
            hostDb.Tenants.Add();
            
            tenantDb.Database.Migrate();

            //stub 2 regular users, 1 admin user
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UserRole>>();

            var newUser = new User { Id = StubUserId, UserName = StubUsername };
            userManager.CreateAsync(newUser, StubPassword).Wait();

            newUser = new User { Id = StubUserId2, UserName = StubUsername2 };
            userManager.CreateAsync(newUser, StubPassword2).Wait();

            newUser = new User
            {
                Id = AdminUserId,
                UserName = AdminUsername,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            roleManager.CreateAsync(new UserRole(UserRoles.Admin)).Wait();
            userManager.CreateAsync(newUser, AdminPassword).Wait();
            userManager.AddToRoleAsync(newUser, UserRoles.Admin).Wait();

            newUser = new User { Id = HighlyReportedUserId, UserName = HighlyReportedUsername };
            db.UserReports.Add(
                new UserReport
                {
                    Id = Guid.NewGuid(),
                    Reason = "Because I can",
                    TargetUserId = HighlyReportedUserId,
                    ReporterId = StubUserId,
                }
            );
            db.UserReports.Add(
                new UserReport
                {
                    Id = Guid.NewGuid(),
                    Reason = "Because I can",
                    TargetUserId = HighlyReportedUserId,
                    ReporterId = StubUserId,
                }
            );
            db.UserReports.Add(
                new UserReport
                {
                    Id = Guid.NewGuid(),
                    Reason = "Because I can",
                    TargetUserId = HighlyReportedUserId,
                    ReporterId = StubUserId,
                }
            );
            db.UserReports.Add(
                new UserReport
                {
                    Id = Guid.NewGuid(),
                    Reason = "Because I can",
                    TargetUserId = HighlyReportedUserId,
                    ReporterId = StubUserId,
                }
            );
            db.UserReports.Add(
                new UserReport
                {
                    Id = Guid.NewGuid(),
                    Reason = "Because I can",
                    TargetUserId = HighlyReportedUserId,
                    ReporterId = StubUserId,
                }
            );
            userManager.CreateAsync(newUser, HighlyReportedPassword).Wait();

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
