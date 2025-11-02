/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write tests for the PostService by being given
///   a description of what exactly should be tested for this service layer and giving
///   back the needed functions and syntax to implement the tests.
///   The code was then reviewed and edited by the author of this file to ensure correctness.
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services;
using Testcontainers.PostgreSql;

namespace OurCity.Api.Test.IntegrationTests;

[Trait("Type", "Integration")]
[Trait("Domain", "Post")]
public class PostIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16.10")
        .Build();
    private AppDbContext _dbContext = null!;
    private User _testUser = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        _dbContext = new AppDbContext(options);

        await _dbContext.Database.EnsureCreatedAsync();

        // Seed a generic user for tests
        _testUser = new User
        {
            Id = 1,
            Username = "Dummy",
            DisplayName = "Dummy",
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _dbContext.Users.Add(_testUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task FreshDbShouldReturnNothing()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var retrievedPost = await postService.GetPosts();

        Assert.True(retrievedPost.IsSuccess);
        Assert.NotNull(retrievedPost.Data);
        Assert.Empty(retrievedPost.Data);
    }

    [Fact]
    public async Task GettingExistingPostByIdShouldReturnPost()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);

        var retrievedPost = await postService.GetPostById(createdPost.Data.Id);
        Assert.True(retrievedPost.IsSuccess);
        Assert.NotNull(retrievedPost.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal(createdPost.Data.Id, retrievedPost.Data.Id);
            Assert.Equal(createdPost.Data.Title, retrievedPost.Data.Title);
            Assert.Equal(_testUser.Id, retrievedPost.Data.AuthorId);
            Assert.Equal(createdPost.Data.Description, retrievedPost.Data.Description);
            Assert.Equal(createdPost.Data.Location, retrievedPost.Data.Location);
            Assert.Equal(createdPost.Data.Votes, retrievedPost.Data.Votes);
            Assert.Equal(
                createdPost.Data.Images.Select(i => i.Url),
                retrievedPost.Data.Images.Select(i => i.Url)
            );
        });
    }

    [Fact]
    public async Task CreatePostShouldAddAndReturnPost()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.True(createdPost.IsSuccess);
        Assert.NotNull(createdPost.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal("Test Post", createdPost.Data.Title);
            Assert.Equal("This is a test post", createdPost.Data.Description);
            Assert.Equal(_testUser.Id, createdPost.Data.AuthorId);
            Assert.Equal(0, createdPost.Data.Votes);
            Assert.Null(createdPost.Data.Location);
            Assert.Empty(createdPost.Data.Images);
        });
    }

    [Fact]
    public async Task UpdatePostShouldModifyAndReturnPost()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);

        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Test Post",
            Description = "This is an updatedPost test post",
            Location = "New Location",
        };

        var updatedPost = await postService.UpdatePost(createdPost.Data.Id, updateDto);

        Assert.True(updatedPost.IsSuccess);
        Assert.NotNull(updatedPost.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal("Updated Test Post", updatedPost.Data.Title);
            Assert.Equal("This is an updatedPost test post", updatedPost.Data.Description);
            Assert.Equal("New Location", updatedPost.Data.Location);
            Assert.Equal(_testUser.Id, updatedPost.Data.AuthorId);
            Assert.Equal(0, updatedPost.Data.Votes);
            Assert.Empty(updatedPost.Data.Images);
        });
    }

    [Fact]
    public async Task UpvoteShouldAdd1Vote()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);
        Assert.Equal(0, createdPost.Data.Votes);

        var userId = 1;
        var upvotedPost = await postService.VotePost(createdPost.Data.Id, userId, VoteType.Upvote);

        Assert.True(upvotedPost.IsSuccess);
        Assert.NotNull(upvotedPost.Data);

        Assert.Equal(1, upvotedPost.Data.Votes);
    }

    [Fact]
    public async Task DownvoteShouldRemove1Vote()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);
        Assert.Equal(0, createdPost.Data.Votes);

        var userId = 2;
        var downvotedPost = await postService.VotePost(
            createdPost.Data.Id,
            userId,
            VoteType.Downvote
        );

        Assert.True(downvotedPost.IsSuccess);
        Assert.NotNull(downvotedPost.Data);

        Assert.Equal(-1, downvotedPost.Data.Votes);
    }

    [Fact]
    public async Task UpvoteShouldNegateDownvote()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);
        Assert.Equal(0, createdPost.Data.Votes);

        var userId = 2;
        var downvotedPost = await postService.VotePost(
            createdPost.Data.Id,
            userId,
            VoteType.Downvote
        );

        Assert.True(downvotedPost.IsSuccess);
        Assert.NotNull(downvotedPost.Data);

        Assert.Equal(-1, downvotedPost.Data.Votes);

        var userIdTwo = 3;
        var upvotedPost = await postService.VotePost(
            createdPost.Data.Id,
            userIdTwo,
            VoteType.Upvote
        );

        Assert.True(upvotedPost.IsSuccess);
        Assert.NotNull(upvotedPost.Data);

        Assert.Equal(0, upvotedPost.Data.Votes);
    }

    [Fact]
    public async Task DownvoteShouldNegateUpvote()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);
        Assert.Equal(0, createdPost.Data.Votes);

        var userId = 2;
        var upvotedPost = await postService.VotePost(createdPost.Data.Id, userId, VoteType.Upvote);

        Assert.True(upvotedPost.IsSuccess);
        Assert.NotNull(upvotedPost.Data);

        Assert.Equal(1, upvotedPost.Data.Votes);

        var userIdTwo = 3;
        var downvotedPost = await postService.VotePost(
            createdPost.Data.Id,
            userIdTwo,
            VoteType.Downvote
        );

        Assert.True(downvotedPost.IsSuccess);
        Assert.NotNull(downvotedPost.Data);

        Assert.Equal(0, downvotedPost.Data.Votes);
    }

    [Fact]
    public async Task DoubleUpvoteShouldNegatePreviousUpvote()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };
        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);
        Assert.Equal(0, createdPost.Data.Votes);

        var userId = 2;
        var upvotedPost = await postService.VotePost(createdPost.Data.Id, userId, VoteType.Upvote);
        Assert.True(upvotedPost.IsSuccess);
        Assert.NotNull(upvotedPost.Data);
        Assert.Equal(1, upvotedPost.Data.Votes);

        var upvotedPostAgain = await postService.VotePost(
            createdPost.Data.Id,
            userId,
            VoteType.Upvote
        );
        Assert.True(upvotedPostAgain.IsSuccess);
        Assert.NotNull(upvotedPostAgain.Data);
        Assert.Equal(0, upvotedPostAgain.Data.Votes);
    }

    [Fact]
    public async Task DoubleDownvoteShouldNegatePreviousDownvote()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };
        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);
        Assert.Equal(0, createdPost.Data.Votes);

        var userId = 2;
        var downvotedPost = await postService.VotePost(
            createdPost.Data.Id,
            userId,
            VoteType.Downvote
        );
        Assert.True(downvotedPost.IsSuccess);
        Assert.NotNull(downvotedPost.Data);
        Assert.Equal(-1, downvotedPost.Data.Votes);

        var downvotedPostAgain = await postService.VotePost(
            createdPost.Data.Id,
            userId,
            VoteType.Downvote
        );
        Assert.True(downvotedPostAgain.IsSuccess);
        Assert.NotNull(downvotedPostAgain.Data);
        Assert.Equal(0, downvotedPostAgain.Data.Votes);
    }

    [Fact]
    public async Task DeletePostShouldRemoveAndReturnPost()
    {
        var postService = new PostService(new PostRepository(_dbContext));
        var createDto = new PostCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Title = "Test Post",
            Description = "This is a test post",
        };

        var createdPost = await postService.CreatePost(createDto);
        Assert.NotNull(createdPost.Data);

        var deletedPost = await postService.DeletePost(createdPost.Data.Id);

        Assert.True(deletedPost.IsSuccess);
        Assert.NotNull(deletedPost.Data);

        var retrievedPost = await postService.GetPostById(deletedPost.Data.Id);
        Assert.False(retrievedPost.IsSuccess);
        Assert.Same(retrievedPost.Error, "Post does not exist");
    }
}
