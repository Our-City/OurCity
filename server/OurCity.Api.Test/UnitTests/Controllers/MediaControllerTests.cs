/// <summary>
/// Tests endpoints in MediaController.cs using mocked service
/// </summary>

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Media;

namespace OurCity.Api.Tests.Controllers;

public class MediaControllerTests
{
    private readonly Mock<IMediaService> _mockMediaService;
    private readonly MediaController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly Guid _testPostId = Guid.NewGuid();
    private readonly Guid _testMediaId = Guid.NewGuid();

    public MediaControllerTests()
    {
        _mockMediaService = new Mock<IMediaService>();
        _controller = new MediaController(_mockMediaService.Object);

        // Setup authenticated user by default
        SetupAuthenticatedUser(_testUserId);
    }

    private void SetupAuthenticatedUser(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    private void SetupAnonymousUser()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // No claims
            }
        };
    }

    private IFormFile CreateMockFormFile(string fileName, long length = 1024)
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        return mockFile.Object;
    }

    #region UploadMedia Tests

    [Fact]
    public async Task UploadMedia_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var file = CreateMockFormFile("test-image.jpg");
        var responseDto = new MediaResponseDto
        {
            Id = _testMediaId,
            PostId = _testPostId,
            Url = "https://test-bucket.s3.amazonaws.com/media/test-image.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediaService
            .Setup(s => s.UploadMediaAsync(
                _testUserId,
                _testPostId,
                It.IsAny<Stream>(),
                file.FileName
            ))
            .ReturnsAsync(Result<MediaResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.UploadMedia(_testPostId, file);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(_controller.UploadMedia), createdResult.ActionName);
        
        var returnedMedia = Assert.IsType<MediaResponseDto>(createdResult.Value);
        Assert.Equal(_testMediaId, returnedMedia.Id);
        Assert.Equal(_testPostId, returnedMedia.PostId);
        
        _mockMediaService.Verify(
            s => s.UploadMediaAsync(
                _testUserId,
                _testPostId,
                It.IsAny<Stream>(),
                file.FileName
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task UploadMedia_WithoutAuthentication_ReturnsProblemDetails()
    {
        // Arrange
        SetupAnonymousUser();
        var file = CreateMockFormFile("test-image.jpg");

        // Act
        var result = await _controller.UploadMedia(_testPostId, file);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("User not authenticated", problemDetails.Detail);
        
        _mockMediaService.Verify(
            s => s.UploadMediaAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Stream>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task UploadMedia_WithNullFile_ReturnsBadRequest()
    {
        // Arrange & Act
        var result = await _controller.UploadMedia(_testPostId, null!);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("No file uploaded", problemDetails.Detail);
        
        _mockMediaService.Verify(
            s => s.UploadMediaAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Stream>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task UploadMedia_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateMockFormFile("test-image.jpg", 0);

        // Act
        var result = await _controller.UploadMedia(_testPostId, file);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("No file uploaded", problemDetails.Detail);
        
        _mockMediaService.Verify(
            s => s.UploadMediaAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<Stream>(),
                It.IsAny<string>()
            ),
            Times.Never
        );
    }

    [Fact]
    public async Task UploadMedia_WithNonExistentPost_ReturnsNotFound()
    {
        // Arrange
        var file = CreateMockFormFile("test-image.jpg");

        _mockMediaService
            .Setup(s => s.UploadMediaAsync(
                _testUserId,
                _testPostId,
                It.IsAny<Stream>(),
                file.FileName
            ))
            .ReturnsAsync(Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound));

        // Act
        var result = await _controller.UploadMedia(_testPostId, file);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.MediaNotFound, problemDetails.Detail);
    }

    [Fact]
    public async Task UploadMedia_WithUnauthorizedUser_ReturnsForbidden()
    {
        // Arrange
        var file = CreateMockFormFile("test-image.jpg");

        _mockMediaService
            .Setup(s => s.UploadMediaAsync(
                _testUserId,
                _testPostId,
                It.IsAny<Stream>(),
                file.FileName
            ))
            .ReturnsAsync(Result<MediaResponseDto>.Failure(ErrorMessages.MediaUnauthorized));

        // Act
        var result = await _controller.UploadMedia(_testPostId, file);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.MediaUnauthorized, problemDetails.Detail);
    }

    #endregion

    #region GetMediaForPost Tests

    [Fact]
    public async Task GetMediaForPost_WithExistingMedia_ReturnsOk()
    {
        // Arrange
        var mediaList = new List<MediaResponseDto>
        {
            new MediaResponseDto
            {
                Id = Guid.NewGuid(),
                PostId = _testPostId,
                Url = "https://test-bucket.s3.amazonaws.com/media/image1.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new MediaResponseDto
            {
                Id = Guid.NewGuid(),
                PostId = _testPostId,
                Url = "https://test-bucket.s3.amazonaws.com/media/image2.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockMediaService
            .Setup(s => s.GetMediaForPostAsync(_testPostId))
            .ReturnsAsync(Result<IEnumerable<MediaResponseDto>>.Success(mediaList));

        // Act
        var result = await _controller.GetMediaForPost(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMedia = Assert.IsType<List<MediaResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedMedia.Count);
        
        _mockMediaService.Verify(
            s => s.GetMediaForPostAsync(_testPostId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetMediaForPost_WithNoMedia_ReturnsEmptyList()
    {
        // Arrange
        _mockMediaService
            .Setup(s => s.GetMediaForPostAsync(_testPostId))
            .ReturnsAsync(Result<IEnumerable<MediaResponseDto>>.Success(new List<MediaResponseDto>()));

        // Act
        var result = await _controller.GetMediaForPost(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMedia = Assert.IsType<List<MediaResponseDto>>(okResult.Value);
        Assert.Empty(returnedMedia);
        
        _mockMediaService.Verify(
            s => s.GetMediaForPostAsync(_testPostId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetMediaForPost_WithAnonymousUser_ReturnsOk()
    {
        // Arrange
        SetupAnonymousUser();
        var mediaList = new List<MediaResponseDto>
        {
            new MediaResponseDto
            {
                Id = Guid.NewGuid(),
                PostId = _testPostId,
                Url = "https://test-bucket.s3.amazonaws.com/media/image1.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockMediaService
            .Setup(s => s.GetMediaForPostAsync(_testPostId))
            .ReturnsAsync(Result<IEnumerable<MediaResponseDto>>.Success(mediaList));

        // Act
        var result = await _controller.GetMediaForPost(_testPostId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMedia = Assert.IsType<List<MediaResponseDto>>(okResult.Value);
        Assert.Single(returnedMedia);
    }

    #endregion

    #region GetMediaById Tests

    [Fact]
    public async Task GetMediaById_WithExistingMedia_ReturnsOk()
    {
        // Arrange
        var mediaDto = new MediaResponseDto
        {
            Id = _testMediaId,
            PostId = _testPostId,
            Url = "https://test-bucket.s3.amazonaws.com/media/image.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediaService
            .Setup(s => s.GetMediaByIdAsync(_testMediaId))
            .ReturnsAsync(Result<MediaResponseDto>.Success(mediaDto));

        // Act
        var result = await _controller.GetMediaById(_testMediaId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMedia = Assert.IsType<MediaResponseDto>(okResult.Value);
        Assert.Equal(_testMediaId, returnedMedia.Id);
        Assert.Equal(_testPostId, returnedMedia.PostId);
        
        _mockMediaService.Verify(
            s => s.GetMediaByIdAsync(_testMediaId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetMediaById_WithNonExistentMedia_ReturnsNotFound()
    {
        // Arrange
        _mockMediaService
            .Setup(s => s.GetMediaByIdAsync(_testMediaId))
            .ReturnsAsync(Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound));

        // Act
        var result = await _controller.GetMediaById(_testMediaId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.MediaNotFound, problemDetails.Detail);
    }

    [Fact]
    public async Task GetMediaById_WithAnonymousUser_ReturnsOk()
    {
        // Arrange
        SetupAnonymousUser();
        var mediaDto = new MediaResponseDto
        {
            Id = _testMediaId,
            PostId = _testPostId,
            Url = "https://test-bucket.s3.amazonaws.com/media/image.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediaService
            .Setup(s => s.GetMediaByIdAsync(_testMediaId))
            .ReturnsAsync(Result<MediaResponseDto>.Success(mediaDto));

        // Act
        var result = await _controller.GetMediaById(_testMediaId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMedia = Assert.IsType<MediaResponseDto>(okResult.Value);
        Assert.Equal(_testMediaId, returnedMedia.Id);
    }

    #endregion

    #region DeleteMedia Tests

    [Fact]
    public async Task DeleteMedia_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var responseDto = new MediaResponseDto
        {
            Id = _testMediaId,
            PostId = _testPostId,
            Url = "https://test-bucket.s3.amazonaws.com/media/image.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediaService
            .Setup(s => s.DeleteMediaAsync(_testUserId, _testMediaId))
            .ReturnsAsync(Result<MediaResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.DeleteMedia(_testMediaId);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        
        _mockMediaService.Verify(
            s => s.DeleteMediaAsync(_testUserId, _testMediaId),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteMedia_WithoutAuthentication_ReturnsProblemDetails()
    {
        // Arrange
        SetupAnonymousUser();

        // Act
        var result = await _controller.DeleteMedia(_testMediaId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("User not authenticated", problemDetails.Detail);
        
        _mockMediaService.Verify(
            s => s.DeleteMediaAsync(It.IsAny<Guid>(), It.IsAny<Guid>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteMedia_WithNonExistentMedia_ReturnsNotFound()
    {
        // Arrange
        _mockMediaService
            .Setup(s => s.DeleteMediaAsync(_testUserId, _testMediaId))
            .ReturnsAsync(Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound));

        // Act
        var result = await _controller.DeleteMedia(_testMediaId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.MediaNotFound, problemDetails.Detail);
    }

    [Fact]
    public async Task DeleteMedia_WithUnauthorizedUser_ReturnsForbidden()
    {
        // Arrange
        _mockMediaService
            .Setup(s => s.DeleteMediaAsync(_testUserId, _testMediaId))
            .ReturnsAsync(Result<MediaResponseDto>.Failure(ErrorMessages.MediaUnauthorized));

        // Act
        var result = await _controller.DeleteMedia(_testMediaId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
        
        var problemDetails = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(ErrorMessages.MediaUnauthorized, problemDetails.Detail);
    }

    #endregion
}