using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services;
using Testcontainers.PostgreSql;

namespace OurCity.Api.Test.IntegrationTests;

/// Generative AI - CoPilot was used to assist in the creation of this file,
/// as it was largely based off of PostIntegrationTests.cs (see that file)
[Trait("Type", "Integration")]
[Trait("Domain", "Comment")]
public class CommentIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16.10")
        .Build();
    private AppDbContext _dbContext = null!; //null! -> tell compiler to trust it will be initialized
    private User _testUser = null!;
    private Post _testPost = null!;

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

        // Seed a generic post for tests
        _testPost = new Post
        {
            Id = 1,
            AuthorId = _testUser.Id,
            Title = "This is a test post",
            Description = "This is a test description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _dbContext.Posts.Add(_testPost);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreateCommentShouldAddAndReturnComment()
    {
        var expectedAuthorId = _testUser.Id;
        var expectedContent = "This is a test comment";

        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = expectedAuthorId,
            Content = expectedContent,
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.True(createdComment.IsSuccess);
        Assert.Multiple(() =>
        {
            Assert.Equal(expectedAuthorId, createdComment.Data?.AuthorId);
            Assert.Equal(expectedContent, createdComment.Data?.Content);
            Assert.Equal(_testPost.Id, createdComment.Data?.PostId);
            Assert.Equal(0, createdComment.Data?.Votes);
            Assert.False(createdComment.Data?.IsDeleted);
        });
    }

    [Fact]
    public async Task GettingExistingCommentByIdShouldReturnComment()
    {
        var expectedAuthorId = _testUser.Id;
        var expectedContent = "This is a test comment";

        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = expectedAuthorId,
            Content = expectedContent,
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);

        var retrievedComment = await commentService.GetCommentById(
            _testPost.Id,
            createdComment.Data.Id
        );
        Assert.True(retrievedComment.IsSuccess);
        Assert.NotNull(retrievedComment.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal(expectedAuthorId, createdComment.Data?.AuthorId);
            Assert.Equal(expectedContent, createdComment.Data?.Content);
            Assert.Equal(_testPost.Id, createdComment.Data?.PostId);
            Assert.Equal(0, createdComment.Data?.Votes);
            Assert.False(createdComment.Data?.IsDeleted);
        });
    }

    [Fact]
    public async Task GettingCommentsByPostIdShouldReturnComments()
    {
        var expectedAuthorId = _testUser.Id;
        var expectedContent = "This is a test comment";

        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = expectedAuthorId,
            Content = expectedContent,
        };

        var createdComment1 = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment1.Data);
        var createdComment2 = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment2.Data);

        var postComments = await commentService.GetCommentsByPostId(_testPost.Id);

        Assert.Equal(2, postComments.Count());

        Assert.All(
            postComments,
            comment =>
                Assert.Multiple(() =>
                {
                    Assert.Equal(expectedAuthorId, comment.AuthorId);
                    Assert.Equal(expectedContent, comment.Content);
                    Assert.Equal(_testPost.Id, comment.PostId);
                    Assert.Equal(0, comment.Votes);
                    Assert.False(comment.IsDeleted);
                })
        );
    }

    [Fact]
    public async Task UpdateCommentShouldModifyAndReturnComment()
    {
        var expectedAuthorId = _testUser.Id;
        var expectedContent = "This is new content";

        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = expectedAuthorId,
            Content = "This is a test comment",
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);

        var updateDto = new CommentUpdateRequestDto { Content = expectedContent };

        var updatedComment = await commentService.UpdateComment(
            _testPost.Id,
            createdComment.Data.Id,
            updateDto
        );
        Assert.True(updatedComment.IsSuccess);
        Assert.NotNull(updatedComment.Data);

        Assert.Multiple(() =>
        {
            Assert.Equal(expectedAuthorId, updatedComment.Data?.AuthorId);
            Assert.Equal(expectedContent, updatedComment.Data?.Content);
            Assert.Equal(_testPost.Id, updatedComment.Data?.PostId);
            Assert.Equal(0, updatedComment.Data?.Votes);
            Assert.False(updatedComment.Data?.IsDeleted);
        });
    }

    [Fact]
    public async Task UpvoteShouldAdd1Vote()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Content = "This is a test comment",
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);
        Assert.Equal(0, createdComment.Data.Votes);

        var userId = 2;
        var upvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Upvote
        );

        Assert.True(upvotedComment.IsSuccess);
        Assert.NotNull(upvotedComment.Data);

        Assert.Equal(1, upvotedComment.Data.Votes);

        var userVoteStatus = await commentService.GetUserUpvoteStatus(
            _testPost.Id,
            createdComment.Data.Id,
            userId
        );
        Assert.True(userVoteStatus.IsSuccess);
        Assert.NotNull(userVoteStatus.Data);
        Assert.True(userVoteStatus.Data.Upvoted);
    }

    [Fact]
    public async Task DownvoteShouldRemove1Vote()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Content = "This is a test comment",
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);
        Assert.Equal(0, createdComment.Data.Votes);

        var userId = 2;
        var downvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Downvote
        );

        Assert.True(downvotedComment.IsSuccess);
        Assert.NotNull(downvotedComment.Data);

        Assert.Equal(-1, downvotedComment.Data.Votes);

        var userVoteStatus = await commentService.GetUserDownvoteStatus(
            _testPost.Id,
            createdComment.Data.Id,
            userId
        );
        Assert.True(userVoteStatus.IsSuccess);
        Assert.NotNull(userVoteStatus.Data);
        Assert.True(userVoteStatus.Data.Downvoted);
    }

    [Fact]
    public async Task UpvoteShouldNegateDownvote()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto()
        {
            AuthorId = _testUser.Id,
            Content = "This is a test comment",
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);
        Assert.Equal(0, createdComment.Data.Votes);

        var userId = 2;
        var downvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Downvote
        );

        Assert.True(downvotedComment.IsSuccess);
        Assert.NotNull(downvotedComment.Data);

        Assert.Equal(-1, downvotedComment.Data.Votes);

        var userIdTwo = 3;
        var upvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userIdTwo,
            VoteType.Upvote
        );

        Assert.True(upvotedComment.IsSuccess);
        Assert.NotNull(upvotedComment.Data);

        Assert.Equal(0, upvotedComment.Data.Votes);
    }

    [Fact]
    public async Task DownvoteShouldNegateUpvote()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto()
        {
            AuthorId = _testUser.Id,
            Content = "This is a test comment",
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);
        Assert.Equal(0, createdComment.Data.Votes);

        var userId = 2;
        var downvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Upvote
        );

        Assert.True(downvotedComment.IsSuccess);
        Assert.NotNull(downvotedComment.Data);

        Assert.Equal(1, downvotedComment.Data.Votes);

        var userIdTwo = 3;
        var upvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userIdTwo,
            VoteType.Downvote
        );

        Assert.True(upvotedComment.IsSuccess);
        Assert.NotNull(upvotedComment.Data);

        Assert.Equal(0, upvotedComment.Data.Votes);
    }

    [Fact]
    public async Task DoubleUpvoteShouldNegatePreviousUpvote()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Content = "This is a test comment",
        };
        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);
        Assert.Equal(0, createdComment.Data.Votes);

        var userId = 2;
        var upvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Upvote
        );
        Assert.True(upvotedComment.IsSuccess);
        Assert.NotNull(upvotedComment.Data);
        Assert.Equal(1, upvotedComment.Data.Votes);

        var upvotedCommentAgain = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Upvote
        );
        Assert.True(upvotedCommentAgain.IsSuccess);
        Assert.NotNull(upvotedCommentAgain.Data);
        Assert.Equal(0, upvotedCommentAgain.Data.Votes);
    }

    [Fact]
    public async Task DoubleDownvoteShouldNegatePreviousDownvote()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Content = "This is a test comment",
        };
        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);
        Assert.Equal(0, createdComment.Data.Votes);

        var userId = 2;
        var downvotedComment = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Downvote
        );
        Assert.True(downvotedComment.IsSuccess);
        Assert.NotNull(downvotedComment.Data);
        Assert.Equal(-1, downvotedComment.Data.Votes);

        var downvotedCommentAgain = await commentService.VoteComment(
            _testPost.Id,
            createdComment.Data.Id,
            userId,
            VoteType.Downvote
        );
        Assert.True(downvotedCommentAgain.IsSuccess);
        Assert.NotNull(downvotedCommentAgain.Data);
        Assert.Equal(0, downvotedCommentAgain.Data.Votes);
    }

    [Fact]
    public async Task DeletePostShouldRemoveAndReturnPost()
    {
        var commentService = new CommentService(new CommentRepository(_dbContext));
        var createDto = new CommentCreateRequestDto
        {
            AuthorId = _testUser.Id,
            Content = "this is a test comment",
        };

        var createdComment = await commentService.CreateComment(_testPost.Id, createDto);
        Assert.NotNull(createdComment.Data);

        var deletedComment = await commentService.DeleteComment(
            _testPost.Id,
            createdComment.Data.Id
        );

        Assert.True(deletedComment.IsSuccess);
        Assert.NotNull(deletedComment.Data);

        var retrievedComment = await commentService.GetCommentById(
            _testPost.Id,
            deletedComment.Data.Id
        );
        Assert.False(retrievedComment.IsSuccess);
        Assert.Same(retrievedComment.Error, "Comment does not exist");
    }
}
