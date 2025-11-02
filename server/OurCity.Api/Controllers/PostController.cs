using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Extensions;
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

    [HttpPost]
    [EndpointSummary("Create a new post")]
    [EndpointDescription("Creates a new post with the provided data")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePost(
        [FromBody] PostCreateRequestDto postCreateRequestDto
    )
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        var res = await _postService.CreatePost(userId.Value, postCreateRequestDto);

        return CreatedAtAction(nameof(GetPosts), new { id = res.Data?.Id }, res.Data);
    }

    [HttpGet]
    [EndpointSummary("Get posts")]
    [EndpointDescription(
        "Gets a list of posts according to the set sort and filter options (if there are any)"
    )]
    [ProducesResponseType(typeof(PaginatedResponseDto<PostResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts([FromQuery] PostGetAllRequestDto postGetAllRequest)
    {
        var userId = User.GetUserId();
        var res = await _postService.GetPosts(userId, postGetAllRequest);

        return Ok(res.Data);
    }

    [HttpGet]
    [Route("{postId}")]
    [EndpointSummary("Get a post by ID")]
    [EndpointDescription("Gets a post by its ID")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] Guid postId)
    {
        var userId = User.GetUserId();
        var res = await _postService.GetPostById(userId, postId);

        if (!res.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: res.Error);
        }

        return Ok(res.Data);
    }

    [HttpPut]
    [Route("{postId}")]
    [EndpointSummary("Update an existing post")]
    [EndpointDescription("Updates an existing post with the provided data")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] Guid postId,
        [FromBody] PostUpdateRequestDto postUpdateRequestDto
    )
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        var res = await _postService.UpdatePost(userId.Value, postId, postUpdateRequestDto);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.PostNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return Ok(res.Data);
    }

    [HttpPut]
    [Route("{postId}/vote")]
    [EndpointSummary("Vote on a post")]
    [EndpointDescription("A user votes on a post, either upvote or downvote")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VotePost(
        [FromRoute] Guid postId,
        [FromBody] PostVoteRequestDto postVoteRequestDto
    )
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        var res = await _postService.VotePost(userId.Value, postId, postVoteRequestDto);

        if (!res.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: res.Error);
        }

        return Ok(res.Data);
    }

    [HttpDelete]
    [Route("{postId}")]
    [EndpointSummary("Delete a post")]
    [EndpointDescription("Deletes a post by its ID")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost([FromRoute] Guid postId)
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        var res = await _postService.DeletePost(userId.Value, postId);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.PostNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return Ok(res.Data);
    }
}
