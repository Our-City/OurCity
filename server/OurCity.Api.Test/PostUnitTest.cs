// /// Generative AI - CoPilot was used to assist in the creation of this file.
// ///   CoPilot was asked to help write unit tests for the Post API methods by being given
// ///   a description of what exactly should be tested for this component and giving
// ///   back the needed syntax to do the test. Mock data was created by CoPilot.
// using Moq;
// using OurCity.Api.Common.Dtos.Post;
// using OurCity.Api.Common.Enum;
// using OurCity.Api.Infrastructure;
// using OurCity.Api.Infrastructure.Database;
// using OurCity.Api.Services;

// [Trait("Type", "Unit")]
// [Trait("Domain", "Post")]
// public class PostUnitTest
// {
//     private readonly Mock<IPostRepository> _mockRepo;
//     private readonly PostService _service;

//     public PostUnitTest()
//     {
//         _mockRepo = new Mock<IPostRepository>();
//         _service = new PostService(_mockRepo.Object);
//     }

//     [Fact]
//     public async Task GetPostsWithExistingPosts()
//     {
//         // Arrange
//         var posts = new List<Post>
//         {
//             new Post
//             {
//                 Id = 1,
//                 Title = "Test Post 1",
//                 Description = "Test Description 1",
//             },
//             new Post
//             {
//                 Id = 2,
//                 Title = "Test Post 2",
//                 Description = "Test Description 2",
//             },
//         };
//         _mockRepo.Setup(r => r.GetAllPosts()).ReturnsAsync(posts as IEnumerable<Post>);

//         // Act
//         var result = await _service.GetPosts();

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(2, result.Data.Count());
//         Assert.Contains(result.Data, p => p.Id == 1);
//     }

//     [Fact]
//     public async Task GetPostsWithNoPosts()
//     {
//         // Arrange
//         _mockRepo.Setup(r => r.GetAllPosts()).ReturnsAsync(new List<Post>() as IEnumerable<Post>);

//         // Act
//         var result = await _service.GetPosts();

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Empty(result.Data);
//     }

//     [Fact]
//     public async Task GetPostByIdWithExistingPost()
//     {
//         // Arrange
//         var post = new Post
//         {
//             Id = 1,
//             Title = "Test Post",
//             Description = "Test Description",
//         };
//         _mockRepo.Setup(r => r.GetPostById(1)).ReturnsAsync(post);

//         // Act
//         var result = await _service.GetPostById(1);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(1, result.Data.Id);
//         Assert.Equal("Test Post", result.Data.Title);
//     }

//     [Fact]
//     public async Task GetPostByIdWithNoExistingPost()
//     {
//         // Arrange
//         _mockRepo.Setup(r => r.GetPostById(99)).ReturnsAsync((Post?)null);

//         // Act
//         var result = await _service.GetPostById(99);

//         // Assert
//         Assert.False(result.IsSuccess);
//         Assert.Null(result.Data);
//         Assert.Equal("Post does not exist", result.Error);
//     }

//     [Fact]
//     public async Task UserDidUpvote()
//     {
//         // Arrange
//         var post = new Post
//         {
//             Id = 1,
//             AuthorId = 10,
//             Title = "Test Post",
//             Description = "Test",
//             UpvotedUserIds = new List<int> { 5 },
//         };
//         _mockRepo.Setup(r => r.GetPostById(1)).ReturnsAsync(post);

//         // Act
//         var result = await _service.GetUserUpvoteStatus(1, 5);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(1, result.Data.Id);
//         Assert.Equal(10, result.Data.AuthorId);
//         Assert.True(result.Data.Upvoted);
//     }

//     [Fact]
//     public async Task UserDidNotUpvote()
//     {
//         // Arrange
//         var post = new Post
//         {
//             Id = 1,
//             AuthorId = 10,
//             Title = "Test Post",
//             Description = "Test",
//             UpvotedUserIds = new List<int> { 7 },
//         };
//         _mockRepo.Setup(r => r.GetPostById(1)).ReturnsAsync(post);

//         // Act
//         var result = await _service.GetUserUpvoteStatus(1, 5);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(1, result.Data.Id);
//         Assert.Equal(10, result.Data.AuthorId);
//         Assert.False(result.Data.Upvoted);
//     }

//     [Fact]
//     public async Task UserUpvoteNotExistingPost()
//     {
//         // Arrange
//         _mockRepo.Setup(r => r.GetPostById(99)).ReturnsAsync((Post?)null);

//         // Act
//         var result = await _service.GetUserUpvoteStatus(99, 5);

//         // Assert
//         Assert.False(result.IsSuccess);
//         Assert.Null(result.Data);
//         Assert.Equal("Post does not exist", result.Error);
//     }

//     [Fact]
//     public async Task UserDidDownvote()
//     {
//         // Arrange
//         var post = new Post
//         {
//             Id = 1,
//             AuthorId = 10,
//             Title = "Test Post",
//             Description = "Test",
//             DownvotedUserIds = new List<int> { 5 },
//         };
//         _mockRepo.Setup(r => r.GetPostById(1)).ReturnsAsync(post);

//         // Act
//         var result = await _service.GetUserDownvoteStatus(1, 5);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(1, result.Data.Id);
//         Assert.Equal(10, result.Data.AuthorId);
//         Assert.True(result.Data.Downvoted);
//     }

//     [Fact]
//     public async Task UserDidNotDownvote()
//     {
//         // Arrange
//         var post = new Post
//         {
//             Id = 1,
//             AuthorId = 10,
//             Title = "Test Post",
//             Description = "Test",
//             DownvotedUserIds = new List<int> { 7 },
//         };
//         _mockRepo.Setup(r => r.GetPostById(1)).ReturnsAsync(post);

//         // Act
//         var result = await _service.GetUserDownvoteStatus(1, 5);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(1, result.Data.Id);
//         Assert.Equal(10, result.Data.AuthorId);
//         Assert.False(result.Data.Downvoted);
//     }

//     [Fact]
//     public async Task UserDownvoteNotExistingPost()
//     {
//         // Arrange
//         _mockRepo.Setup(r => r.GetPostById(99)).ReturnsAsync((Post?)null);

//         // Act
//         var result = await _service.GetUserDownvoteStatus(99, 5);

//         // Assert
//         Assert.False(result.IsSuccess);
//         Assert.Null(result.Data);
//         Assert.Equal("Post does not exist", result.Error);
//     }

//     [Fact]
//     public async Task CreatePostWhenValid()
//     {
//         // Arrange
//         var dto = new PostCreateRequestDto
//         {
//             AuthorId = 10,
//             Title = "Test Post",
//             Description = "Test Description",
//         };
//         var post = new Post
//         {
//             Id = 1,
//             AuthorId = 10,
//             Title = "Test Post",
//             Description = "Test Description",
//         };
//         _mockRepo.Setup(r => r.CreatePost(It.IsAny<Post>())).ReturnsAsync(post);

//         // Act
//         var result = await _service.CreatePost(dto);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(1, result.Data.Id);
//         Assert.Equal("Test Post", result.Data.Title);
//         Assert.Equal("Test Description", result.Data.Description);
//         Assert.Equal(10, result.Data.AuthorId);
//     }

//     [Fact]
//     public async Task UpdatePost_ReturnsUpdatedPost_WhenValid()
//     {
//         // Arrange
//         var postId = 1;
//         var updateDto = new PostUpdateRequestDto
//         {
//             Title = "Updated Title",
//             Description = "Updated Description",
//         };
//         var existingPost = new Post
//         {
//             Id = postId,
//             Title = "Old Title",
//             Description = "Old Description",
//         };
//         var updatedPost = new Post
//         {
//             Id = postId,
//             Title = "Updated Title",
//             Description = "Updated Description",
//         };

//         _mockRepo.Setup(r => r.GetPostById(postId)).ReturnsAsync(existingPost);
//         _mockRepo.Setup(r => r.UpdatePost(It.IsAny<Post>())).ReturnsAsync(updatedPost);

//         // Act
//         var result = await _service.UpdatePost(postId, updateDto);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(postId, result.Data.Id);
//         Assert.Equal("Updated Title", result.Data.Title);
//         Assert.Equal("Updated Description", result.Data.Description);
//     }

//     [Fact]
//     public async Task VotePost_ReturnsUpdatedPost_WhenValid()
//     {
//         // Arrange
//         var postId = 1;
//         var userId = 42;
//         var voteType = VoteType.Upvote;
//         var existingPost = new Post
//         {
//             Id = postId,
//             Title = "Test Post",
//             Description = "Test",
//             UpvotedUserIds = new List<int>(),
//         };
//         var updatedPost = new Post
//         {
//             Id = postId,
//             Title = "Test Post",
//             Description = "Test",
//             UpvotedUserIds = new List<int> { userId },
//         };

//         _mockRepo.Setup(r => r.GetPostById(postId)).ReturnsAsync(existingPost);
//         _mockRepo.Setup(r => r.UpdatePost(It.IsAny<Post>())).ReturnsAsync(updatedPost);

//         // Act
//         var result = await _service.VotePost(postId, userId, voteType);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(postId, result.Data.Id);
//         Assert.Contains(userId, updatedPost.UpvotedUserIds);
//     }

//     [Fact]
//     public async Task DeletePost_ReturnsDeletedPost_WhenValid()
//     {
//         // Arrange
//         var postId = 1;
//         var existingPost = new Post
//         {
//             Id = postId,
//             Title = "Test Post",
//             Description = "Test",
//         };

//         _mockRepo.Setup(r => r.GetPostById(postId)).ReturnsAsync(existingPost);
//         _mockRepo.Setup(r => r.DeletePost(existingPost)).ReturnsAsync(existingPost);

//         // Act
//         var result = await _service.DeletePost(postId);

//         // Assert
//         Assert.True(result.IsSuccess);
//         Assert.NotNull(result.Data);
//         Assert.Equal(postId, result.Data.Id);
//         Assert.Equal("Test Post", result.Data.Title);
//     }
// }
