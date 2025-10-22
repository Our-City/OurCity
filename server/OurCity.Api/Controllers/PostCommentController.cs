/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API development
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Contracts.v1.Dtos;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("posts/{postId}/comments")] // comments are sub-resource of posts
public class PostCommentController : ControllerBase
{
    private readonly ILogger<PostCommentController> _logger;

    public PostCommentController(ILogger<PostCommentController> logger)
    {
        _logger = logger;
    }
/*
    [HttpGet]
    [EndpointSummary("Get all comments associated with a post")]
    [EndpointDescription("Gets a list of all comments for a specific post")]
    [ProducesResponseType(typeof(List<PostCommentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommentsForPost([FromRoute] int postId)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{commentId}/upvote/{userId}")]
    [EndpointSummary("Get a user's upvote status for a comment")]
    [ProducesResponseType(typeof(PostCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyVoteStatus(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromRoute] int userId
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [EndpointSummary("Create a new comment under a post")]
    [EndpointDescription("Creates a new comment to be associated with a specific post")]
    [ProducesResponseType(typeof(PostCommentDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateComment(
        [FromRoute] int postId,
        [FromBody] CommentCreateRequestDto commentCreateRequestDto
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut("{commentId}")]
    [EndpointSummary("Update an existing comment")]
    [EndpointDescription("Updates an existing comment associated with a specific post")]
    [ProducesResponseType(typeof(PostCommentDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateComment(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromBody] CommentUpdateRequestDto commentUpdateRequestDto
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [Route("{commentId}/vote")]
    [EndpointSummary("Vote on a comment")]
    [EndpointDescription("A user votes on a comment, either upvote or downvote")]
    [ProducesResponseType(typeof(PostCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VoteComment(
        [FromRoute] int postId,
        [FromRoute] int commentId,
        [FromBody] CommentVoteRequestDto commentVoteRequestDto
    )
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }*/
}