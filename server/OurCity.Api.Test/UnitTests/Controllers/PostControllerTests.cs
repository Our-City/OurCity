/// <summary>
/// Tests endpoints in PostController.cs using mocked service
/// </summary>
/// <credits>
/// Claude ai was used to help generate the boilerplate for this file.
/// Prompt: Given a method from the controller, implement the endpoint test cases with thorough coverage.
/// </credits>
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Controllers;
using OurCity.Api.Services;

namespace OurCity.Api.Test.UnitTests.Controllers;

[Trait("Type", "Unit")]
[Trait("Domain", "Post")]
public class PostControllerTests
{
    private readonly Mock<IPostService> _mockPostService;
    private readonly Mock<ILogger<PostController>> _mockLogger;
    private readonly PostController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly Guid _testPostId = Guid.NewGuid();
    private readonly int _defaultPaginationLimit = 25;

    public PostControllerTests()
    {
        _mockPostService = new Mock<IPostService>();
        _mockLogger = new Mock<ILogger<PostController>>();
        _controller = new PostController(_mockPostService.Object, _mockLogger.Object);

        // Setup authenticated user by default
        SetupAuthenticatedUser(_testUserId);
    }

    private void SetupAuthenticatedUser(Guid userId)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth")),
            },
        };
    }

    private void SetupAnonymousUser()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()), // No claims
            },
        };
    }

    #region CreatePost Tests

    [Fact]
    public async Task CreatePost_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new PostCreateRequestDto
        {
            Title = "Test Post",
            Description = "Test Description",
        };

        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = _testUserId,
            Title = "Test Post",
            Description = "Test Description",
            UpvoteCount = 0,
            DownvoteCount = 0,
            VoteStatus = VoteType.NoVote,
            Tags = [],
            IsDeleted = false,
            CanMutate = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.CreatePost(createDto))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.CreatePost(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(_controller.GetPosts), createdResult.ActionName);

        var returnedPost = Assert.IsType<PostResponseDto>(createdResult.Value);
        Assert.Equal(_testPostId, returnedPost.Id);
        Assert.Equal("Test Post", returnedPost.Title);

        _mockPostService.Verify(s => s.CreatePost(createDto), Times.Once);
    }

    #endregion

    #region GetPosts Tests

    [Fact]
    public async Task GetPosts_WithAuthenticatedUser_ReturnsPostsWithVoteStatus()
    {
        // Arrange
        var posts = new List<PostResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AuthorId = _testUserId,
                Title = "Post 1",
                Description = "Description 1",
                VoteStatus = VoteType.Upvote,
                Tags = [],
                UpvoteCount = 1,
                DownvoteCount = 0,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Title = "Post 2",
                Description = "Description 2",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 5,
                DownvoteCount = 2,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = posts,
            NextCursor = null,
        };

        _mockPostService
            .Setup(s => s.GetPosts(postGetAllRequestDto))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act
        var result = await _controller.GetPosts(postGetAllRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedData.Items.Count());
        Assert.Null(returnedData.NextCursor);

        _mockPostService.Verify(s => s.GetPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithAnonymousUser_ReturnsPostsWithoutVoteStatus()
    {
        // Arrange
        SetupAnonymousUser();
        var posts = new List<PostResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Title = "Post 1",
                Description = "Description 1",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 10,
                DownvoteCount = 3,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = posts,
            NextCursor = null,
        };

        _mockPostService
            .Setup(s => s.GetPosts(postGetAllRequestDto))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act
        var result = await _controller.GetPosts(postGetAllRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        Assert.Single(returnedData.Items);
        Assert.Null(returnedData.NextCursor);

        _mockPostService.Verify(s => s.GetPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithNoPosts_ReturnsEmptyList()
    {
        // Arrange
        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = new List<PostResponseDto>(),
            NextCursor = null,
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        _mockPostService
            .Setup(s => s.GetPosts(postGetAllRequestDto))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act
        var result = await _controller.GetPosts(postGetAllRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        Assert.Empty(returnedData.Items);
        Assert.Null(returnedData.NextCursor);

        _mockPostService.Verify(s => s.GetPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithCursor_ReturnsNextPage()
    {
        // Arrange
        var cursor = Guid.NewGuid();
        var nextCursor = Guid.NewGuid();
        var posts = new List<PostResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AuthorId = _testUserId,
                Title = "Post 3",
                Description = "Description 3",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 2,
                DownvoteCount = 1,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = cursor,
            Limit = _defaultPaginationLimit,
        };

        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = posts,
            NextCursor = nextCursor,
        };

        _mockPostService
            .Setup(s => s.GetPosts(postGetAllRequestDto))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act
        var result = await _controller.GetPosts(postGetAllRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        Assert.Single(returnedData.Items);
        Assert.Equal(nextCursor, returnedData.NextCursor);

        _mockPostService.Verify(s => s.GetPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithCustomLimit_ReturnsSpecifiedNumberOfPosts()
    {
        // Arrange
        var customLimit = 10;
        var posts = Enumerable
            .Range(1, customLimit)
            .Select(i => new PostResponseDto
            {
                Id = Guid.NewGuid(),
                AuthorId = _testUserId,
                Title = $"Post {i}",
                Description = $"Description {i}",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 0,
                DownvoteCount = 0,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            })
            .ToList();

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = posts,
            NextCursor = Guid.NewGuid(),
        };

        _mockPostService
            .Setup(s => s.GetPosts(postGetAllRequestDto))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act
        var result = await _controller.GetPosts(postGetAllRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        Assert.Equal(customLimit, returnedData.Items.Count());
        Assert.NotNull(returnedData.NextCursor);

        _mockPostService.Verify(s => s.GetPosts(postGetAllRequestDto), Times.Once);
    }

    [Fact]
    public async Task GetPosts_WithDefaultLimit_UsesDefaultPaginationLimit()
    {
        // Arrange
        var posts = new List<PostResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AuthorId = _testUserId,
                Title = "Post 1",
                Description = "Description 1",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 0,
                DownvoteCount = 0,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        var postGetAllRequestDto = new PostGetAllRequestDto
        {
            Cursor = null,
            Limit = _defaultPaginationLimit,
        };

        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = posts,
            NextCursor = null,
        };

        _mockPostService
            .Setup(s => s.GetPosts(postGetAllRequestDto))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act - Not passing limit parameter, should use default (25)
        var result = await _controller.GetPosts(postGetAllRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        Assert.Single(returnedData.Items);

        _mockPostService.Verify(s => s.GetPosts(postGetAllRequestDto), Times.Once);
    }

    #endregion

    #region GetPostById Tests

    [Fact]
    public async Task GetPostById_WithExistingPost_ReturnsOk()
    {
        // Arrange
        var postDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = _testUserId,
            Title = "Test Post",
            Description = "Test Description",
            VoteStatus = VoteType.Upvote,
            Tags = [],
            UpvoteCount = 5,
            DownvoteCount = 1,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.GetPostById(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Success(postDto));

        // Act
        var result = await _controller.GetPostById(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(_testPostId, returnedPost.Id);
        Assert.Equal("Test Post", returnedPost.Title);

        _mockPostService.Verify(s => s.GetPostById(_testPostId), Times.Once);
    }

    [Fact]
    public async Task GetPostById_WithNonExistentPost_ReturnsProblemDetails()
    {
        // Arrange
        _mockPostService
            .Setup(s => s.GetPostById(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound));

        // Act
        var result = await _controller.GetPostById(_testPostId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.PostNotFound, problemDetails.Detail);
    }

    [Fact]
    public async Task GetPostById_WithAnonymousUser_ReturnsPost()
    {
        // Arrange
        SetupAnonymousUser();
        var postDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = Guid.NewGuid(),
            Title = "Public Post",
            Description = "Public Description",
            VoteStatus = VoteType.NoVote,
            Tags = [],
            UpvoteCount = 10,
            DownvoteCount = 2,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.GetPostById(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Success(postDto));

        // Act
        var result = await _controller.GetPostById(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(VoteType.NoVote, returnedPost.VoteStatus);
    }

    #endregion

    #region UpdatePost Tests

    [Fact]
    public async Task UpdatePost_WithValidData_ReturnsOk()
    {
        // Arrange
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = _testUserId,
            Title = "Updated Title",
            Description = "Updated Description",
            VoteStatus = VoteType.NoVote,
            Tags = [],
            UpvoteCount = 0,
            DownvoteCount = 0,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.UpdatePost(_testPostId, updateDto))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.UpdatePost(_testPostId, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal("Updated Title", returnedPost.Title);
        Assert.Equal("Updated Description", returnedPost.Description);

        _mockPostService.Verify(s => s.UpdatePost(_testPostId, updateDto), Times.Once);
    }

    [Fact]
    public async Task UpdatePost_WithNonExistentPost_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        _mockPostService
            .Setup(s => s.UpdatePost(_testPostId, updateDto))
            .ReturnsAsync(Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound));

        // Act
        var result = await _controller.UpdatePost(_testPostId, updateDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.PostNotFound, problemDetails.Detail);
    }

    [Fact]
    public async Task UpdatePost_WithDifferentAuthor_ReturnsForbidden()
    {
        // Arrange
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        _mockPostService
            .Setup(s => s.UpdatePost(_testPostId, updateDto))
            .ReturnsAsync(Result<PostResponseDto>.Failure(ErrorMessages.PostUnauthorized));

        // Act
        var result = await _controller.UpdatePost(_testPostId, updateDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.PostUnauthorized, problemDetails.Detail);
    }

    #endregion

    #region VotePost Tests

    [Fact]
    public async Task VotePost_WithUpvote_ReturnsOk()
    {
        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };

        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = Guid.NewGuid(),
            Title = "Test Post",
            Description = "Test Description",
            VoteStatus = VoteType.Upvote,
            Tags = [],
            UpvoteCount = 1,
            DownvoteCount = 0,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.VotePost(_testPostId, voteDto))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.VotePost(_testPostId, voteDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(VoteType.Upvote, returnedPost.VoteStatus);
        Assert.Equal(1, returnedPost.UpvoteCount);

        _mockPostService.Verify(s => s.VotePost(_testPostId, voteDto), Times.Once);
    }

    [Fact]
    public async Task VotePost_WithDownvote_ReturnsOk()
    {
        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Downvote };

        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = Guid.NewGuid(),
            Title = "Test Post",
            Description = "Test Description",
            VoteStatus = VoteType.Downvote,
            Tags = [],
            UpvoteCount = 0,
            DownvoteCount = 1,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.VotePost(_testPostId, voteDto))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.VotePost(_testPostId, voteDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(VoteType.Downvote, returnedPost.VoteStatus);
        Assert.Equal(1, returnedPost.DownvoteCount);
    }

    [Fact]
    public async Task VotePost_WithNoVote_ReturnsOk()
    {
        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.NoVote };

        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = Guid.NewGuid(),
            Title = "Test Post",
            Description = "Test Description",
            VoteStatus = VoteType.NoVote,
            Tags = [],
            UpvoteCount = 0,
            DownvoteCount = 0,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.VotePost(_testPostId, voteDto))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.VotePost(_testPostId, voteDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(VoteType.NoVote, returnedPost.VoteStatus);
    }

    [Fact]
    public async Task VotePost_WithNonExistentPost_ReturnsNotFound()
    {
        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };

        _mockPostService
            .Setup(s => s.VotePost(_testPostId, voteDto))
            .ReturnsAsync(Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound));

        // Act
        var result = await _controller.VotePost(_testPostId, voteDto);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.PostNotFound, problemDetails.Detail);
    }

    #endregion

    #region DeletePost Tests

    [Fact]
    public async Task DeletePost_WithValidData_ReturnsOk()
    {
        // Arrange
        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = _testUserId,
            Title = "Deleted Post",
            Description = "Deleted Description",
            VoteStatus = VoteType.NoVote,
            Tags = [],
            UpvoteCount = 0,
            DownvoteCount = 0,
            IsDeleted = true,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _mockPostService
            .Setup(s => s.DeletePost(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.DeletePost(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.True(returnedPost.IsDeleted);

        _mockPostService.Verify(s => s.DeletePost(_testPostId), Times.Once);
    }

    [Fact]
    public async Task DeletePost_WithNonExistentPost_ReturnsNotFound()
    {
        // Arrange
        _mockPostService
            .Setup(s => s.DeletePost(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound));

        // Act
        var result = await _controller.DeletePost(_testPostId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.PostNotFound, problemDetails.Detail);
    }

    [Fact]
    public async Task DeletePost_WithDifferentAuthor_ReturnsForbidden()
    {
        // Arrange
        _mockPostService
            .Setup(s => s.DeletePost(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Failure(ErrorMessages.PostUnauthorized));

        // Act
        var result = await _controller.DeletePost(_testPostId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);

        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.PostUnauthorized, problemDetails.Detail);
    }

    #endregion

    #region BookmarkPosts Tests

    [Fact]
    public async Task BookmarkPost_WithAuthenticatedUser_ReturnsOk()
    {
        var responseDto = new PostResponseDto
        {
            Id = _testPostId,
            AuthorId = Guid.NewGuid(),
            Title = "Test Post",
            Description = "Test Description",
            VoteStatus = VoteType.NoVote,
            Tags = [],
            UpvoteCount = 0,
            DownvoteCount = 0,
            IsDeleted = false,
            CanMutate = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        // Arrange
        _mockPostService
            .Setup(s => s.BookmarkPost(_testPostId))
            .ReturnsAsync(Result<PostResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.BookmarkPost(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPost = Assert.IsType<PostResponseDto>(okResult.Value);
        Assert.Equal(_testPostId, returnedPost.Id);
        Assert.Equal("Test Post", returnedPost.Title);

        _mockPostService.Verify(s => s.BookmarkPost(_testPostId), Times.Once);
    }

    #endregion

    #region GetBookmarkedPosts Tests

    [Fact]
    public async Task GetBookmarkedPosts_WithAuthenticatedUser_ReturnsPosts()
    {
        // Arrange
        var posts = new List<PostResponseDto>
        {
            new PostResponseDto
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Title = "Test Post 1",
                Description = "Test Description 1",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 0,
                DownvoteCount = 0,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new PostResponseDto
            {
                Id = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Title = "Test Post 2",
                Description = "Test Description 2",
                VoteStatus = VoteType.NoVote,
                Tags = [],
                UpvoteCount = 0,
                DownvoteCount = 0,
                IsDeleted = false,
                CanMutate = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        var paginatedResponse = new PaginatedResponseDto<PostResponseDto>
        {
            Items = posts,
            NextCursor = null,
        };

        _mockPostService
            .Setup(s => s.GetBookmarkedPosts(null, 25))
            .ReturnsAsync(Result<PaginatedResponseDto<PostResponseDto>>.Success(paginatedResponse));

        // Act
        var result = await _controller.GetBookmarkedPosts(null, 25);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<PaginatedResponseDto<PostResponseDto>>(okResult.Value);
        var returnedPosts = returnedData.Items.ToList();
        Assert.Equal(2, returnedPosts.Count);
        Assert.Equal("Test Post 1", returnedPosts[0].Title);
        Assert.Equal("Test Post 2", returnedPosts[1].Title);
        Assert.Null(returnedData.NextCursor);

        _mockPostService.Verify(s => s.GetBookmarkedPosts(null, 25), Times.Once);
    }

    #endregion
}
