using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("[controller]s")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;
    private readonly IPostService _postService;

    public PostController(IPostService postService, ILogger<PostController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet]
    [EndpointSummary("Get all posts")]
    [EndpointDescription("Gets a list of all posts")]
    [ProducesResponseType(typeof(List<PostResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _postService.GetPosts();

        return Ok(posts);
    }

    [HttpGet]
    [Route("{postId}")]
    [EndpointSummary("Get a post by ID")]
    [EndpointDescription("Gets a post by its ID")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] int postId)
    {
        var post = await _postService.GetPostById(postId);

        if (!post.IsSuccess)
        {
            return NotFound(post.Error);
        }

        return Ok(post.Data);
    }

    [HttpGet]
    [Route("{postId}/upvote/{userId}")]
    [EndpointSummary("Get a user's upvote status for a post")]
    [ProducesResponseType(typeof(PostUpvoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserUpvoteStatus(
        [FromRoute] int postId,
        [FromRoute] int userId
    )
    {
        var voteStatus = await _postService.GetUserUpvoteStatus(postId, userId);

        if (!voteStatus.IsSuccess)
        {
            return NotFound(voteStatus.Error);
        }

        return Ok(voteStatus.Data);
    }

    [HttpGet]
    [Route("{postId}/downvote/{userId}")]
    [EndpointSummary("Get a user's downvote status for a post")]
    [ProducesResponseType(typeof(PostDownvoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserDownvoteStatus(
        [FromRoute] int postId,
        [FromRoute] int userId
    )
    {
        var voteStatus = await _postService.GetUserDownvoteStatus(postId, userId);

        if (!voteStatus.IsSuccess)
        {
            return NotFound(voteStatus.Error);
        }

        return Ok(voteStatus.Data);
    }

    [HttpPost]
    [EndpointSummary("Create a new post")]
    [EndpointDescription("Creates a new post with the provided data")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePost(
        [FromBody] PostCreateRequestDto postCreateRequestDto
    )
    {
        var post = await _postService.CreatePost(postCreateRequestDto);

        return CreatedAtAction(nameof(GetPosts), new { id = post.Data?.Id }, post.Data);
    }

    [HttpPut]
    [Route("{postId}")]
    [EndpointSummary("Update an existing post")]
    [EndpointDescription("Updates an existing post with the provided data")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] int postId,
        [FromBody] PostUpdateRequestDto postUpdateRequestDto
    )
    {
        var post = await _postService.UpdatePost(postId, postUpdateRequestDto);

        if (!post.IsSuccess)
        {
            return NotFound(post.Error);
        }

        return Ok(post.Data);
    }

    [HttpPut]
    [Route("{postId}/vote")]
    [EndpointSummary("Vote on a post")]
    [EndpointDescription("A user votes on a post, either upvote or downvote")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VotePost(
        [FromRoute] int postId,
        [FromBody] PostVoteRequestDto postVoteRequestDto
    )
    {
        var post = await _postService.VotePost(
            postId,
            postVoteRequestDto.UserId,
            postVoteRequestDto.VoteType
        );

        if (!post.IsSuccess)
        {
            return NotFound(post.Error);
        }

        return Ok(post.Data);
    }

    [HttpDelete]
    [Route("{postId}")]
    [EndpointSummary("Delete a post")]
    [EndpointDescription("Deletes a post by its ID")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost([FromRoute] int postId)
    {
        var post = await _postService.DeletePost(postId);

        if (!post.IsSuccess)
        {
            return NotFound(post.Error);
        }

        return Ok(post.Data);
    }
}
