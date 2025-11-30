using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("apis/v1/posts")]
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
    [Authorize]
    [EndpointSummary("Create a new post")]
    [EndpointDescription("Creates a new post with the provided data")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePost(
        [FromBody] PostCreateRequestDto postCreateRequestDto
    )
    {
        var res = await _postService.CreatePost(postCreateRequestDto);

        if (!res.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, detail: res.Error);
        }

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
        var res = await _postService.GetPosts(postGetAllRequest);

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
        var res = await _postService.GetPostById(postId);

        if (!res.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: res.Error);
        }

        return Ok(res.Data);
    }

    [HttpGet]
    [Route("bookmarks")]
    [EndpointSummary("Get bookmarked posts")]
    [EndpointDescription("Retrieves all posts bookmarked by the authenticated user")]
    [ProducesResponseType(typeof(IEnumerable<PostResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBookmarkedPosts(
        [FromQuery] Guid? cursor,
        [FromQuery] int limit = 25
    )
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: ErrorMessages.UserNotAuthenticated
            );
        }

        var res = await _postService.GetBookmarkedPosts(userId.Value, cursor, limit);
        return Ok(res.Data);
    }

    [HttpPut]
    [Authorize]
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
        var res = await _postService.UpdatePost(postId, postUpdateRequestDto);

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
    [Authorize]
    [Route("{postId}/votes")]
    [EndpointSummary("Vote on a post")]
    [EndpointDescription("A user votes on a post, either upvote or downvote")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VotePost(
        [FromRoute] Guid postId,
        [FromBody] PostVoteRequestDto postVoteRequestDto
    )
    {
        var res = await _postService.VotePost(postId, postVoteRequestDto);

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
    [Route("{postId}/bookmarks")]
    [EndpointSummary("Bookmark a post")]
    [EndpointDescription("Bookmarks/saves a post for the authenticated user")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BookmarkPost([FromRoute] Guid postId)
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: ErrorMessages.UserNotAuthenticated
            );
        }

        var res = await _postService.BookmarkPost(userId.Value, postId);

        if (!res.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: res.Error);
        }

        return Ok(res.Data);
    }

    [HttpDelete]
    [Authorize]
    [Route("{postId}")]
    [EndpointSummary("Delete a post")]
    [EndpointDescription("Deletes a post by its ID")]
    [ProducesResponseType(typeof(PostResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost([FromRoute] Guid postId)
    {
        var res = await _postService.DeletePost(postId);

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
