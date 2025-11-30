using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Test.IntegrationTests;

[Trait("Type", "Integration")]
[Trait("Domain", "Post")]
public class PostIntegrationTests : IClassFixture<OurCityWebApplicationFactory>, IAsyncLifetime
{
    private OurCityWebApplicationFactory _ourCityApi = null!;
    private Guid _testPostId;
    private Guid _testTagId;
    private readonly string _baseUrl = "/apis/v1";

    public async Task InitializeAsync()
    {
        _ourCityApi = new OurCityWebApplicationFactory();
        await _ourCityApi.StartDbAsync();
        await _ourCityApi.ResetDatabaseAsync();
        await SeedTestData();
    }

    public async Task DisposeAsync()
    {
        await _ourCityApi.StopDbAsync();
    }

    private async Task SeedTestData()
    {
        await _ourCityApi.SeedTestDataAsync(
            async (db, userManager) =>
            {
                _testPostId = Guid.NewGuid();
                _testTagId = Guid.NewGuid();

                var tag = new Tag { Id = _testTagId, Name = "Test Tag" };
                db.Tags.Add(tag);
                await db.SaveChangesAsync();

                var post = new Post
                {
                    Id = _testPostId,
                    AuthorId = _ourCityApi.StubUserId,
                    Title = "Test Post",
                    Description = "Test Description",
                    Location = "Test Location",
                    Visisbility = PostVisibility.Published,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Tags = new List<Tag> { tag },
                };
                db.Posts.Add(post);
            }
        );
    }

    #region GetPosts Tests

    [Fact]
    public async Task GetPosts_WithNoPosts_ReturnsEmptyList()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange - Reset without seeding
        await _ourCityApi.ResetDatabaseAsync();

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Null(result.NextCursor);
    }

    [Fact]
    public async Task GetPosts_WithExistingPosts_ReturnsPaginatedPosts()
    {
        using var client = _ourCityApi.CreateClient();

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Test Post", result.Items.First().Title);
    }

    [Fact]
    public async Task GetPosts_WithSearchTerm_ReturnsMatchingPosts()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange - Add another post
        await _ourCityApi.SeedTestDataAsync(
            async (db, userManager) =>
            {
                await db.Posts.AddAsync(
                    new Post
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = _ourCityApi.StubUserId,
                        Title = "React Tutorial",
                        Description = "Learn React basics",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    }
                );
            }
        );

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts?searchTerm=React");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Contains("React", result.Items.First().Title);
    }

    [Fact]
    public async Task GetPosts_WithTagFilter_ReturnsPostsWithMatchingTags()
    {
        using var client = _ourCityApi.CreateClient();

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts?tags={_testTagId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Contains(result.Items.First().Tags, t => t.Id == _testTagId);
    }

    [Fact]
    public async Task GetPosts_WithSortByVotes_ReturnsSortedPosts()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        await _ourCityApi.SeedTestDataAsync(
            async (db, userManager) =>
            {
                var post1Id = Guid.NewGuid();
                var post2Id = Guid.NewGuid();

                var post1 = new Post
                {
                    Id = post1Id,
                    AuthorId = _ourCityApi.StubUserId,
                    Title = "Low Votes Post",
                    Description = "Description",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                var post2 = new Post
                {
                    Id = post2Id,
                    AuthorId = _ourCityApi.StubUserId,
                    Title = "High Votes Post",
                    Description = "Description",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                db.Posts.AddRange(post1, post2);
                await db.SaveChangesAsync();

                db.PostVotes.Add(
                    new PostVote
                    {
                        Id = Guid.NewGuid(),
                        PostId = post2Id,
                        VoterId = _ourCityApi.StubUserId,
                        VoteType = VoteType.Upvote,
                        VotedAt = DateTime.UtcNow,
                    }
                );
            }
        );

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts?sortBy=votes&sortOrder=Desc");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.True(result.Items.Count() >= 2);
        Assert.Equal("High Votes Post", result.Items.First().Title);
    }

    [Fact]
    public async Task GetPosts_WithPagination_ReturnsCorrectPage()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange - Create 30 posts
        await _ourCityApi.SeedTestDataAsync(
            async (db, userManager) =>
            {
                for (int i = 0; i < 30; i++)
                {
                    await db.Posts.AddAsync(
                        new Post
                        {
                            Id = Guid.NewGuid(),
                            AuthorId = _ourCityApi.StubUserId,
                            Title = $"Post {i}",
                            Description = $"Description {i}",
                            CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                            UpdatedAt = DateTime.UtcNow,
                        }
                    );
                }
            }
        );

        // Act - Get first page
        var response1 = await client.GetAsync($"{_baseUrl}/posts?limit=10");
        response1.EnsureSuccessStatusCode();
        var result1 = await response1.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();

        Assert.NotNull(result1);
        Assert.Equal(10, result1.Items.Count());
        Assert.NotNull(result1.NextCursor);

        // Act - Get second page using cursor
        var response2 = await client.GetAsync(
            $"{_baseUrl}/posts?limit=10&cursor={result1.NextCursor}"
        );
        response2.EnsureSuccessStatusCode();
        var result2 = await response2.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();

        Assert.NotNull(result2);
        Assert.Equal(10, result2.Items.Count());

        // Verify no duplicate posts
        var firstPageIds = result1.Items.Select(p => p.Id).ToList();
        var secondPageIds = result2.Items.Select(p => p.Id).ToList();
        Assert.Empty(firstPageIds.Intersect(secondPageIds));
    }

    #endregion

    #region GetPostById Tests

    [Fact]
    public async Task GetPostById_WithExistingPost_ReturnsPost()
    {
        using var client = _ourCityApi.CreateClient();

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts/{_testPostId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var post = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(post);
        Assert.Equal(_testPostId, post.Id);
        Assert.Equal("Test Post", post.Title);
    }

    [Fact]
    public async Task GetPostById_WithNonExistentPost_ReturnsNotFound()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region CreatePost Tests

    [Fact]
    public async Task CreatePost_WithoutLogin_ReturnUnauthorized()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var createDto = new PostCreateRequestDto
        {
            Title = "Test Title",
            Description = "Description Description",
        };

        // Act
        var response = await client.PostAsJsonAsync($"{_baseUrl}/posts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_WithValidData_SavesPostToDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var createDto = new PostCreateRequestDto
        {
            Title = "New Post",
            Description = "New Description for test",
            Location = "Test Location",
            Tags = new List<Guid> { _testTagId },
        };

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PostAsJsonAsync($"{_baseUrl}/posts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(createdPost);
        Assert.Equal(createDto.Title, createdPost.Title);
        Assert.Single(createdPost.Tags);

        // Verify in database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var savedPost = await db
            .Posts.Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Title == createDto.Title);
        Assert.NotNull(savedPost);
        Assert.Equal(createDto.Description, savedPost.Description);
        Assert.Single(savedPost.Tags);
    }

    [Fact]
    public async Task CreatePost_WithInvalidData_ReturnsBadRequest()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var createDto = new PostCreateRequestDto
        {
            Title = "", // Invalid - empty title
            Description = "Description",
        };

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PostAsJsonAsync($"{_baseUrl}/posts", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region UpdatePost Tests

    [Fact]
    public async Task UpdatePost_WithoutLogin_ReturnUnauthorized()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        // Act
        var response = await client.PutAsJsonAsync($"{_baseUrl}/posts/{_testPostId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_AsNonAuthor_ReturnForbidden()
    {
        using var client = _ourCityApi.CreateClient();

        var nonAuthorUsername = "testUser";
        var nonAuthorPassword = "TestPassword1!";

        // Arrange
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        await _ourCityApi.SeedTestDataAsync(
            async (db, userManager) =>
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = nonAuthorUsername,
                    CreatedAt = DateTime.UtcNow,
                };
                await userManager.CreateAsync(user, nonAuthorPassword);
            }
        );

        var loginRequest = new UserCreateRequestDto
        {
            Username = nonAuthorUsername,
            Password = nonAuthorPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PutAsJsonAsync($"{_baseUrl}/posts/{_testPostId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePost_WithValidData_UpdatesPostInDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var updateDto = new PostUpdateRequestDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
        };

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PutAsJsonAsync($"{_baseUrl}/posts/{_testPostId}", updateDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(updatedPost);
        Assert.Equal(updateDto.Title, updatedPost.Title);

        // Verify in database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var savedPost = await db.Posts.FindAsync(_testPostId);
        Assert.NotNull(savedPost);
        Assert.Equal(updateDto.Title, savedPost.Title);
        Assert.Equal(updateDto.Description, savedPost.Description);
    }

    [Fact]
    public async Task UpdatePost_WithNonExistentPost_ReturnsNotFound()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new PostUpdateRequestDto { Title = "Updated" };

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PutAsJsonAsync($"{_baseUrl}/posts/{nonExistentId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region VotePost Tests

    [Fact]
    public async Task VotePost_WithoutLogin_ReturnUnauthorized()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };

        // Act
        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/posts/{_testPostId}/votes",
            voteDto
        );

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task VotePost_WithUpvote_SavesVoteToDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var voteDto = new PostVoteRequestDto { VoteType = VoteType.Upvote };
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/posts/{_testPostId}/votes",
            voteDto
        );

        // Assert
        response.EnsureSuccessStatusCode();
        var votedPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(votedPost);
        Assert.Equal(1, votedPost.UpvoteCount);
        Assert.Equal(VoteType.Upvote, votedPost.VoteStatus);

        // Verify in database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var vote = await db.PostVotes.FirstOrDefaultAsync(v => v.PostId == _testPostId);
        Assert.NotNull(vote);
        Assert.Equal(VoteType.Upvote, vote.VoteType);
    }

    [Fact]
    public async Task VotePost_WithNoVote_RemovesVoteFromDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        await client.PutAsJsonAsync(
            $"{_baseUrl}/posts/{_testPostId}/votes",
            new PostVoteRequestDto { VoteType = VoteType.Upvote }
        );

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/posts/{_testPostId}/votes",
            new PostVoteRequestDto { VoteType = VoteType.NoVote }
        );

        // Assert
        response.EnsureSuccessStatusCode();
        var votedPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(votedPost);
        Assert.Equal(0, votedPost.UpvoteCount);

        // Verify vote removed from database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var vote = await db.PostVotes.FirstOrDefaultAsync(v => v.PostId == _testPostId);
        Assert.Null(vote);
    }

    [Fact]
    public async Task VotePost_ChangingVote_UpdatesVoteInDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        // Arrange - First vote upvote
        await client.PutAsJsonAsync(
            $"{_baseUrl}/posts/{_testPostId}/votes",
            new PostVoteRequestDto { VoteType = VoteType.Upvote }
        );

        // Act - Change to downvote
        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/posts/{_testPostId}/votes",
            new PostVoteRequestDto { VoteType = VoteType.Downvote }
        );

        // Assert
        response.EnsureSuccessStatusCode();
        var votedPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(votedPost);
        Assert.Equal(0, votedPost.UpvoteCount);
        Assert.Equal(1, votedPost.DownvoteCount);

        // Verify in database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var vote = await db.PostVotes.FirstOrDefaultAsync(v => v.PostId == _testPostId);
        Assert.NotNull(vote);
        Assert.Equal(VoteType.Downvote, vote.VoteType);
    }

    #endregion

    #region DeletePost Tests

    [Fact]
    public async Task DeletePost_WithoutLogin_ReturnUnauthorized()
    {
        using var client = _ourCityApi.CreateClient();

        // Act
        var response = await client.DeleteAsync($"{_baseUrl}/posts/{_testPostId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_AsNonAuthor_ReturnForbidden()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var nonAuthorUsername = "testUser";
        var nonAuthorPassword = "TestPassword1!";

        await _ourCityApi.SeedTestDataAsync(
            async (db, userManager) =>
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = nonAuthorUsername,
                    CreatedAt = DateTime.UtcNow,
                };
                await userManager.CreateAsync(user, nonAuthorPassword);
            }
        );

        var loginRequest = new UserCreateRequestDto
        {
            Username = nonAuthorUsername,
            Password = nonAuthorPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.DeleteAsync($"{_baseUrl}/posts/{_testPostId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_WithValidPost_MarksPostAsDeletedInDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.DeleteAsync($"{_baseUrl}/posts/{_testPostId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var deletedPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(deletedPost);
        Assert.True(deletedPost.IsDeleted);

        // Verify in database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var savedPost = await db.Posts.FindAsync(_testPostId);
        Assert.NotNull(savedPost);
        Assert.True(savedPost.IsDeleted);
    }

    [Fact]
    public async Task DeletePost_WithNonExistentPost_ReturnsNotFound()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var nonExistentId = Guid.NewGuid();
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.DeleteAsync($"{_baseUrl}/posts/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region BookmarkPost Tests

    [Fact]
    public async Task BookmarkPost_WithoutLogin_ReturnUnauthorized()
    {
        using var client = _ourCityApi.CreateClient();

        // Act
        var response = await client.PutAsync($"{_baseUrl}/posts/{_testPostId}/bookmarks", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task BookmarkPost_WithValidPost_SavesBookmarkToDatabase()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PutAsync($"{_baseUrl}/posts/{_testPostId}/bookmarks", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var bookmarkedPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(bookmarkedPost);

        // Verify in database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookmark = await db.PostBookmarks.FirstOrDefaultAsync(b => b.PostId == _testPostId);
        Assert.NotNull(bookmark);
    }

    [Fact]
    public async Task BookmarkPost_TogglingBookmark_RemovesBookmarkFromDatabase()
    {
        using var client = _ourCityApi.CreateClient();
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        await client.PutAsync($"{_baseUrl}/posts/{_testPostId}/bookmarks", null);

        var response = await client.PutAsync($"{_baseUrl}/posts/{_testPostId}/bookmarks", null);

        // Assert
        response.EnsureSuccessStatusCode();
        var toggledPost = await response.Content.ReadFromJsonAsync<PostResponseDto>();
        Assert.NotNull(toggledPost);

        // Verify bookmark removed from database
        using var scope = _ourCityApi.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var bookmark = await db.PostBookmarks.FirstOrDefaultAsync(b => b.PostId == _testPostId);
        Assert.Null(bookmark);
    }

    [Fact]
    public async Task BookmarkPost_WithNonExistentPost_ReturnsNotFound()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var nonExistentId = Guid.NewGuid();
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        // Act
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        var response = await client.PutAsync($"{_baseUrl}/posts/{nonExistentId}/bookmarks", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region GetBookmarkedPosts Tests

    [Fact]
    public async Task GetBookmarkedPosts_WithoutLogin_ReturnUnauthorized()
    {
        using var client = _ourCityApi.CreateClient();

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts/bookmarks");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetBookmarkedPosts_WithBookmarks_ReturnsBookmarkedPosts()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
        await client.PutAsync($"{_baseUrl}/posts/{_testPostId}/bookmarks", null);

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts/bookmarks");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(_testPostId, result.Items.First().Id);
    }

    [Fact]
    public async Task GetBookmarkedPosts_WithoutBookmarks_ReturnsEmptyList()
    {
        using var client = _ourCityApi.CreateClient();

        // Arrange
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        // Act
        var response = await client.GetAsync($"{_baseUrl}/posts/bookmarks");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<PostResponseDto>
        >();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    #endregion
}
