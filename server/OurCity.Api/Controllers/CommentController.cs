/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API development
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("Posts/{postId}/[controller]s")] // comments are sub-resource of posts
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpGet]
    [EndpointSummary("Get all comments associated with a post")]
    [EndpointDescription("Gets a list of all comments for a specific post")]
    [ProducesResponseType(typeof(List<CommentResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComments([FromRoute] int postId)
    {
        var comments = await _commentService.GetCommentsByPostId(postId);
        return Ok(comments);
    }

    [HttpGet("{commentId}")]
    [EndpointSummary("Get a specific comment associated with a post")]
    [EndpointDescription("Gets a specific comment for a specific post")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComment([FromRoute] int postId, [FromRoute] int commentId)
    {
        var comment = await _commentService.GetCommentById(postId, commentId);
        if (comment == null)
            return NotFound();

        return Ok(comment);
    }

    [HttpGet]
    [Route("{commentId}/upvote/{userId}")]
    [EndpointSummary("Get a user's upvote status for a comment")]
    [ProducesResponseType(typeof(CommentUpvoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserUpvoteStatus(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromRoute] int userId
    )
    {
        var voteStatus = await _commentService.GetUserUpvoteStatus(postId, commentId, userId);

        if (!voteStatus.IsSuccess)
        {
            return NotFound(voteStatus.Error);
        }

        return Ok(voteStatus.Data);
    }

    [HttpGet]
    [Route("{commentId}/downvote/{userId}")]
    [EndpointSummary("Get a user's downvote status for a comment")]
    [ProducesResponseType(typeof(CommentDownvoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserDownvoteStatus(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromRoute] int userId
    )
    {
        var voteStatus = await _commentService.GetUserDownvoteStatus(postId, commentId, userId);

        if (!voteStatus.IsSuccess)
        {
            return NotFound(voteStatus.Error);
        }

        return Ok(voteStatus.Data);
    }

    [HttpPost]
    [EndpointSummary("Create a new comment under a post")]
    [EndpointDescription("Creates a new comment to be associated with a specific post")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateComment(
        [FromRoute] int postId,
        [FromBody] CommentCreateRequestDto commentCreateRequestDto
    )
    {
        var comment = await _commentService.CreateComment(postId, commentCreateRequestDto);

        return CreatedAtAction(
            nameof(GetComment),
            new { postId = comment.Data?.PostId, commentId = comment.Data?.Id },
            comment.Data
        );
    }

    [HttpPut("{commentId}")]
    [EndpointSummary("Update an existing comment")]
    [EndpointDescription("Updates an existing comment associated with a specific post")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateComment(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromBody] CommentUpdateRequestDto commentUpdateRequestDto
    )
    {
        var comment = await _commentService.UpdateComment(
            postId,
            commentId,
            commentUpdateRequestDto
        );

        if (comment.Data == null)
            return NotFound();

        return Ok(comment.Data);
    }

    [HttpPut]
    [Route("{commentId}/vote")]
    [EndpointSummary("Vote on a comment")]
    [EndpointDescription("A user votes on a comment, either upvote or downvote")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VoteComment(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromBody] CommentVoteRequestDto commentVoteRequestDto
    )
    {
        var comment = await _commentService.VoteComment(
            postId,
            commentId,
            commentVoteRequestDto.UserId,
            commentVoteRequestDto.VoteType
        );

        if (!comment.IsSuccess)
        {
            return NotFound(comment.Error);
        }

        return Ok(comment.Data);
    }

    [HttpDelete("{commentId}")]
    [EndpointSummary("Delete a comment")]
    [EndpointDescription("Deletes a comment associated with a specific post")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] int postId,
        [FromRoute] int commentId
    )
    {
        var comment = await _commentService.DeleteComment(postId, commentId);

        if (comment.Data == null)
            return NotFound();

        return Ok(comment.Data);
    }
}
