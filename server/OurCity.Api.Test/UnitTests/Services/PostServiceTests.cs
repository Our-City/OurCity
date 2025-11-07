/// <summary>
/// Tests endpoints in PostService.cs using mocked repository
/// </summary>
/// <credits>
/// Claude ai was used to help mocking some repository layer code
/// Prompt: Help me mock creating a Post while ensuring my logic for converting the dto into an entity is ran before we get a mocked result
/// </credits>
using Moq;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services;

namespace OurCity.Api.Test.UnitTests.Services;

[Trait("Type", "Unit")]
[Trait("Domain", "Post")]
public class PostServiceTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly Mock<IPostVoteRepository> _mockPostVoteRepository;
    private readonly PostService _service;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly Guid _testPostId = Guid.NewGuid();
    private readonly int _defaultPaginationLimit = 25;

    public PostServiceTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _mockTagRepository = new Mock<ITagRepository>();
        _mockPostVoteRepository = new Mock<IPostVoteRepository>();

        _service = new PostService(
            _mockPostRepository.Object,
            _mockTagRepository.Object,
            _mockPostVoteRepository.Object
        );
    }

    #region GetPosts Tests

    [Fact]
    public async Task GetPosts_WithNoPosts_ReturnsEmptyPaginatedResponse()
    {
        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        // Arrange
        _mockPostRepository
            .Setup(r => r.GetAllPosts(postGetAllRequestDto))
            .ReturnsAsync(new List<Post>());

        // Act
        var result = await _service.GetPosts(_testUserId, postGetAllRequestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);
        Assert.Null(result.Data.NextCursor);

        _mockPostRepository.Verify(r => r.GetAllPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithLessThanLimit_ReturnsAllPostsWithoutNextCursor()
    {
        // Arrange
        var posts = new List<Post>
        {
            CreateTestFatPost(_testPostId, _testUserId, "Post 1"),
            CreateTestFatPost(Guid.NewGuid(), _testUserId, "Post 2"),
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        _mockPostRepository.Setup(r => r.GetAllPosts(postGetAllRequestDto)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetPosts(_testUserId, postGetAllRequestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Null(result.Data.NextCursor);
    }

    [Fact]
    public async Task GetPosts_WithMoreThanLimit_ReturnsLimitedPostsWithNextCursor()
    {
        // Arrange
        var posts = new List<Post>();
        for (int i = 0; i < _defaultPaginationLimit + 1; i++)
        {
            posts.Add(CreateTestFatPost(Guid.NewGuid(), _testUserId, $"Post {i}"));
        }

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        _mockPostRepository.Setup(r => r.GetAllPosts(postGetAllRequestDto)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetPosts(_testUserId, postGetAllRequestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(_defaultPaginationLimit, result.Data.Items.Count());
        Assert.NotNull(result.Data.NextCursor);
        Assert.Equal(posts[_defaultPaginationLimit - 1].Id, result.Data.NextCursor);
    }

    [Fact]
    public async Task GetPosts_WithCursor_PassesCursorToRepository()
    {
        // Arrange
        var cursor = Guid.NewGuid();
        var posts = new List<Post> { CreateTestFatPost(Guid.NewGuid(), _testUserId, "Post 1") };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = cursor,
            Limit = _defaultPaginationLimit,
        };

        _mockPostRepository.Setup(r => r.GetAllPosts(postGetAllRequestDto)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetPosts(_testUserId, postGetAllRequestDto);

        // Assert
        Assert.True(result.IsSuccess);
        _mockPostRepository.Verify(r => r.GetAllPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithAuthenticatedUser_ReturnsPostsWithVoteStatus()
    {
        // Arrange
        var posts = new List<Post>
        {
            CreateTestPostWithVote(_testPostId, _testUserId, Guid.NewGuid(), VoteType.Upvote),
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        _mockPostRepository.Setup(r => r.GetAllPosts(postGetAllRequestDto)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetPosts(_testUserId, postGetAllRequestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var firstPost = result.Data.Items.First();
        Assert.Equal(VoteType.Upvote, firstPost.VoteStatus);
    }

    [Fact]
    public async Task GetPosts_WithAnonymousUser_ReturnsPostsWithNoVoteStatus()
    {
        // Arrange
        var posts = new List<Post> { CreateTestFatPost(_testPostId, Guid.NewGuid(), "Post 1") };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        _mockPostRepository.Setup(r => r.GetAllPosts(postGetAllRequestDto)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetPosts(null, postGetAllRequestDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        var firstPost = result.Data.Items.First();
        Assert.Equal(VoteType.NoVote, firstPost.VoteStatus);
    }

    #endregion

    #region GetPostById Tests

    [Fact]
    public async Task GetPostById_WithExistingPost_ReturnsSuccess()
    {
        // Arrange
        var post = CreateTestFatPost(_testPostId, _testUserId, "Test Post");

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        // Act
        var result = await _service.GetPostById(_testUserId, _testPostId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(_testPostId, result.Data.Id);
        Assert.Equal("Test Post", result.Data.Title);

        _mockPostRepository.Verify(r => r.GetFatPostById(_testPostId), Times.Once);
    }

    [Fact]
    public async Task GetPostById_WithNonExistentPost_ReturnsFailure()
    {
        // Arrange
        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync((Post?)null);

        // Act
        var result = await _service.GetPostById(_testUserId, _testPostId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessages.PostNotFound, result.Error);
    }

    [Fact]
    public async Task GetPostById_WithAuthenticatedUser_ReturnsPostWithVoteStatus()
    {
        // Arrange
        var post = CreateTestPostWithVote(
            _testPostId,
            _testUserId,
            Guid.NewGuid(),
            VoteType.Upvote
        );

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        // Act
        var result = await _service.GetPostById(_testUserId, _testPostId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(VoteType.Upvote, result.Data.VoteStatus);
    }

    [Fact]
    public async Task GetPostById_WithAnonymousUser_ReturnsPostWithNoVoteStatus()
    {
        // Arrange
        var post = CreateTestFatPost(_testPostId, Guid.NewGuid(), "Test Post");

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        // Act
        var result = await _service.GetPostById(null, _testPostId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(VoteType.NoVote, result.Data.VoteStatus);
    }

    #endregion

    #region CreatePost Tests

    [Fact]
    public async Task CreatePost_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var createDto = new PostCreateRequestDto
        {
            Title = "Test Post",
            Description = "Test Description",
            Location = "Test Location",
            TagIds = new List<Guid>(),
        };

        var tags = new List<Tag>();
        var createdPost = CreateTestFatPost(Guid.NewGuid(), _testUserId, createDto.Title);

        _mockTagRepository.Setup(r => r.GetTagsByIds(createDto.TagIds)).ReturnsAsync(tags);

        _mockPostRepository.Setup(r => r.CreatePost(It.IsAny<Post>())).ReturnsAsync(createdPost);

        // Act
        var result = await _service.CreatePost(_testUserId, createDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(createDto.Title, result.Data.Title);
        Assert.Equal(createDto.Description, result.Data.Description);

        _mockTagRepository.Verify(r => r.GetTagsByIds(createDto.TagIds), Times.Once);
        _mockPostRepository.Verify(r => r.CreatePost(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task CreatePost_WithTags_AssociatesTagsWithPost()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var tags = new List<Tag>
        {
            new Tag { Id = tagId, Name = "TestTag" },
        };

        var createDto = new PostCreateRequestDto
        {
            Title = "Post with Tags",
            Description = "Description",
            TagIds = new List<Guid> { tagId },
        };

        Post? capturedPost = null;

        _mockTagRepository.Setup(r => r.GetTagsByIds(createDto.TagIds)).ReturnsAsync(tags);

        _mockPostRepository
            .Setup(r => r.CreatePost(It.IsAny<Post>()))
            .Callback<Post>(p => capturedPost = p)
            .ReturnsAsync((Post p) => p);

        // Act
        var result = await _service.CreatePost(_testUserId, createDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedPost);
        Assert.Single(capturedPost.Tags);
        Assert.Equal(tagId, capturedPost.Tags.First().Id);
    }

    [Fact]
    public async Task CreatePost_SetsDefaultValues()
    {
        // Arrange
        var createDto = new PostCreateRequestDto
        {
            Title = "New Post",
            Description = "Description",
        };

        Post? capturedPost = null;
        _mockTagRepository
            .Setup(r => r.GetTagsByIds(It.IsAny<List<Guid>>()))
            .ReturnsAsync(new List<Tag>());

        _mockPostRepository
            .Setup(r => r.CreatePost(It.IsAny<Post>()))
            .Callback<Post>(p => capturedPost = p)
            .ReturnsAsync((Post p) => p);

        // Act
        var result = await _service.CreatePost(_testUserId, createDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedPost);
        Assert.Equal(_testUserId, capturedPost.AuthorId);
        Assert.Equal(PostVisibility.Published, capturedPost.Visisbility);
        Assert.False(capturedPost.IsDeleted);
        Assert.NotEqual(Guid.Empty, capturedPost.Id);
    }

    #endregion

    #region UpdatePost Tests

    [Fact]
    public async Task UpdatePost_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var post = CreateTestFatPost(_testPostId, _testUserId, "Original Title");
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdatePost(_testUserId, _testPostId, updateDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(updateDto.Title, result.Data.Title);
        Assert.Equal(updateDto.Description, result.Data.Description);

        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdatePost_WithNonExistentPost_ReturnsFailure()
    {
        // Arrange
        var updateDto = new PostUpdateRequestDto { Title = "Updated" };

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync((Post?)null);

        // Act
        var result = await _service.UpdatePost(_testUserId, _testPostId, updateDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessages.PostNotFound, result.Error);

        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdatePost_WithDifferentUser_ReturnsUnauthorized()
    {
        // Arrange
        var differentUserId = Guid.NewGuid();
        var post = CreateTestFatPost(_testPostId, _testUserId, "Test Post");
        var updateDto = new PostUpdateRequestDto { Title = "Updated" };

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        // Act
        var result = await _service.UpdatePost(differentUserId, _testPostId, updateDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessages.PostUnauthorized, result.Error);

        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdatePost_WithPartialData_OnlyUpdatesProvidedFields()
    {
        // Arrange
        var post = CreateTestFatPost(_testPostId, _testUserId, "Original Title");
        post.Description = "Original Description";
        post.Location = "Original Location";

        var updateDto = new PostUpdateRequestDto { Title = "Updated Title" };

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdatePost(_testUserId, _testPostId, updateDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(updateDto.Title, result.Data.Title);
        Assert.Equal(post.Description, result.Data.Description);
        Assert.Equal(post.Location, result.Data.Location);
    }

    [Fact]
    public async Task UpdatePost_WithNewTags_UpdatesTags()
    {
        // Arrange
        var oldTagId = Guid.NewGuid();
        var newTagId = Guid.NewGuid();

        var post = CreateTestFatPost(_testPostId, _testUserId, "Test Post");
        post.Tags = new List<Tag>
        {
            new Tag { Id = oldTagId, Name = "OldTag" },
        };

        var updateDto = new PostUpdateRequestDto { TagIds = new List<Guid> { newTagId } };

        var newTags = new List<Tag>
        {
            new Tag { Id = newTagId, Name = "NewTag" },
        };

        _mockPostRepository.Setup(r => r.GetFatPostById(_testPostId)).ReturnsAsync(post);

        _mockTagRepository.Setup(r => r.GetTagsByIds(updateDto.TagIds)).ReturnsAsync(newTags);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdatePost(_testUserId, _testPostId, updateDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data.Tags);
        Assert.Equal(newTags[0].Id, result.Data.Tags[0].Id);
        Assert.Equal(newTags[0].Name, result.Data.Tags[0].Name);

        _mockTagRepository.Verify(r => r.GetTagsByIds(updateDto.TagIds), Times.Once);
    }

    #endregion

    #region VotePost Tests

    [Fact]
    public async Task VotePost_WithNoExistingVote_CreatesNewVote()
    {
        // Arrange
        var post = CreateTestSlimPost(_testPostId, Guid.NewGuid(), "Test Post");
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        _mockPostVoteRepository
            .Setup(r => r.GetVoteByPostAndUserId(_testPostId, _testUserId))
            .ReturnsAsync((PostVote?)null);

        _mockPostVoteRepository.Setup(r => r.Add(It.IsAny<PostVote>())).Returns(Task.CompletedTask);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.VotePost(_testUserId, _testPostId, voteDto);

        // Assert
        Assert.True(result.IsSuccess);

        _mockPostVoteRepository.Verify(
            r =>
                r.Add(
                    It.Is<PostVote>(v =>
                        v.PostId == _testPostId
                        && v.VoterId == _testUserId
                        && v.VoteType == VoteType.Upvote
                    )
                ),
            Times.Once
        );
        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task VotePost_WithNoVote_RemovesVote()
    {
        // Arrange
        var post = CreateTestSlimPost(_testPostId, Guid.NewGuid(), "Test Post");
        var existingVote = new PostVote
        {
            Id = Guid.NewGuid(),
            PostId = _testPostId,
            VoterId = _testUserId,
            VoteType = VoteType.Upvote,
            VotedAt = DateTime.UtcNow,
        };

        var voteDto = new PostVoteRequestDto { VoteType = VoteType.NoVote };

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        _mockPostVoteRepository
            .Setup(r => r.GetVoteByPostAndUserId(_testPostId, _testUserId))
            .ReturnsAsync(existingVote);

        _mockPostVoteRepository.Setup(r => r.Remove(existingVote)).Returns(Task.CompletedTask);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.VotePost(_testUserId, _testPostId, voteDto);

        // Assert
        Assert.True(result.IsSuccess);

        _mockPostVoteRepository.Verify(r => r.Remove(existingVote), Times.Once);
        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task VotePost_WithExistingDifferentVote_UpdatesVote()
    {
        // Arrange
        var post = CreateTestSlimPost(_testPostId, Guid.NewGuid(), "Test Post");
        var existingVote = new PostVote
        {
            Id = Guid.NewGuid(),
            PostId = _testPostId,
            VoterId = _testUserId,
            VoteType = VoteType.Upvote,
            VotedAt = DateTime.UtcNow.AddHours(-1),
        };

        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Downvote };

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        _mockPostVoteRepository
            .Setup(r => r.GetVoteByPostAndUserId(_testPostId, _testUserId))
            .ReturnsAsync(existingVote);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.VotePost(_testUserId, _testPostId, voteDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(VoteType.Downvote, existingVote.VoteType);

        _mockPostVoteRepository.Verify(r => r.Add(It.IsAny<PostVote>()), Times.Never);
        _mockPostVoteRepository.Verify(r => r.Remove(It.IsAny<PostVote>()), Times.Never);
        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task VotePost_WithNonExistentPost_ReturnsFailure()
    {
        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync((Post?)null);

        // Act
        var result = await _service.VotePost(_testUserId, _testPostId, voteDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessages.PostNotFound, result.Error);

        _mockPostVoteRepository.Verify(r => r.Add(It.IsAny<PostVote>()), Times.Never);
        _mockPostVoteRepository.Verify(r => r.Remove(It.IsAny<PostVote>()), Times.Never);
    }

    [Fact]
    public async Task VotePost_UpdatesPostUpdatedAt()
    {
        // Arrange
        var post = CreateTestSlimPost(_testPostId, Guid.NewGuid(), "Test Post");
        var originalUpdatedAt = post.UpdatedAt;
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        _mockPostVoteRepository
            .Setup(r => r.GetVoteByPostAndUserId(_testPostId, _testUserId))
            .ReturnsAsync((PostVote?)null);

        _mockPostVoteRepository.Setup(r => r.Add(It.IsAny<PostVote>())).Returns(Task.CompletedTask);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await Task.Delay(10); // Ensure time difference
        var result = await _service.VotePost(_testUserId, _testPostId, voteDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(post.UpdatedAt > originalUpdatedAt);
    }

    #endregion

    #region DeletePost Tests

    [Fact]
    public async Task DeletePost_WithValidData_MarksPostAsDeleted()
    {
        // Arrange
        var post = CreateTestSlimPost(_testPostId, _testUserId, "Test Post");

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeletePost(_testUserId, _testPostId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.IsDeleted);
        Assert.True(post.IsDeleted);

        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeletePost_WithNonExistentPost_ReturnsFailure()
    {
        // Arrange
        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync((Post?)null);

        // Act
        var result = await _service.DeletePost(_testUserId, _testPostId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessages.PostNotFound, result.Error);

        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletePost_WithDifferentUser_ReturnsUnauthorized()
    {
        // Arrange
        var differentUserId = Guid.NewGuid();
        var post = CreateTestSlimPost(_testPostId, _testUserId, "Test Post");

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        // Act
        var result = await _service.DeletePost(differentUserId, _testPostId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessages.PostUnauthorized, result.Error);
        Assert.False(post.IsDeleted);

        _mockPostRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletePost_UpdatesPostUpdatedAt()
    {
        // Arrange
        var post = CreateTestSlimPost(_testPostId, _testUserId, "Test Post");
        var originalUpdatedAt = post.UpdatedAt;

        _mockPostRepository.Setup(r => r.GetSlimPostbyId(_testPostId)).ReturnsAsync(post);

        _mockPostRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await Task.Delay(10); // Ensure time difference
        var result = await _service.DeletePost(_testUserId, _testPostId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(post.UpdatedAt > originalUpdatedAt);
    }

    #endregion

    #region Helper Methods

    private Post CreateTestFatPost(Guid id, Guid authorId, string title)
    {
        return new Post
        {
            Id = id,
            AuthorId = authorId,
            Title = title,
            Description = "Test Description",
            Location = "Test Location",
            Visisbility = PostVisibility.Published,
            Tags = new List<Tag>(),
            Votes = new List<PostVote>(),
            Comments = new List<Comment>(),
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    private Post CreateTestSlimPost(Guid id, Guid authorId, string title)
    {
        return new Post
        {
            Id = id,
            AuthorId = authorId,
            Title = title,
            Description = "Test Description",
            Location = "Test Location",
            Visisbility = PostVisibility.Published,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    private Post CreateTestPostWithVote(Guid postId, Guid voterId, Guid authorId, VoteType voteType)
    {
        var post = CreateTestFatPost(postId, authorId, "Test Post");
        post.Votes = new List<PostVote>
        {
            new PostVote
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                VoterId = voterId,
                VoteType = voteType,
                VotedAt = DateTime.UtcNow,
            },
        };
        return post;
    }

    #endregion
}
