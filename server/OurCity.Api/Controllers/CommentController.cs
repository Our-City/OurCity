/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API development
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Extensions;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("apis/v1")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpPost]
    [Route("posts/{postId}/comments")]
    [EndpointSummary("Create a new comment under a post")]
    [EndpointDescription("Creates a new comment to be associated with a specific post")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateComment(
        [FromRoute] Guid postId,
        [FromBody] CommentRequestDto commentRequestDto
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

        var res = await _commentService.CreateComment(userId.Value, postId, commentRequestDto);

        return StatusCode(StatusCodes.Status201Created, res.Data);
    }

    [HttpGet]
    [Route("posts/{postId}/comments")]
    [EndpointSummary("Get all comments associated with a post")]
    [EndpointDescription("Gets a list of all comments for a specific post")]
    [ProducesResponseType(
        typeof(PaginatedResponseDto<CommentResponseDto>),
        StatusCodes.Status200OK
    )]
    public async Task<IActionResult> GetCommentsForPost(
        [FromRoute] Guid postId,
        [FromQuery] Guid? cursor,
        [FromQuery] int limit = 25
    )
    {
        var userId = User.GetUserId();
        var result = await _commentService.GetCommentsForPost(userId, postId, cursor, limit);
        return Ok(result.Data);
    }

    [HttpPut]
    [Route("comments/{commentId}")]
    [EndpointSummary("Update an existing comment")]
    [EndpointDescription("Updates an existing comment associated with a specific post")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateComment(
        [FromRoute] Guid commentId,
        [FromBody] CommentRequestDto commentRequestDto
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

        var res = await _commentService.UpdateComment(userId.Value, commentId, commentRequestDto);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.CommentNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return Ok(res.Data);
    }

    [HttpPut]
    [Route("comments/{commentId}/votes")]
    [EndpointSummary("Vote on a comment")]
    [EndpointDescription("A user votes on a comment, either upvote or downvote")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VoteComment(
        [FromRoute] Guid commentId,
        [FromBody] CommentVoteRequestDto commentVoteRequestDto
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

        var res = await _commentService.VoteComment(userId.Value, commentId, commentVoteRequestDto);

        if (!res.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: res.Error);
        }

        return Ok(res.Data);
    }

    [HttpDelete("comments/{commentId}")]
    [EndpointSummary("Delete a comment")]
    [EndpointDescription("Deletes a comment associated with a specific post")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId)
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        var res = await _commentService.DeleteComment(userId.Value, commentId);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.CommentNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return Ok(res.Data);
    }
}
