using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Contracts.v1.Dtos;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("posts")]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> _logger;

    public PostController(ILogger<PostController> logger)
    {
        _logger = logger;
    }
/*
    [HttpGet]
    [EndpointSummary("Get all posts")]
    [EndpointDescription("Gets a list of all posts")]
    [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{postId}")]
    [EndpointSummary("Get a post by ID")]
    [EndpointDescription("Gets a post by its ID")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] int postId)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{postId}/upvote/{userId}")]
    [EndpointSummary("Get a user's upvote status for a post")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserVoteStatus(
        [FromRoute] int postId,
        [FromRoute] int userId
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [EndpointSummary("Create a new post")]
    [EndpointDescription("Creates a new post with the provided data")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreatePost(
        [FromBody] PostCreateRequestDto postCreateRequestDto
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [Route("{postId}")]
    [EndpointSummary("Update an existing post")]
    [EndpointDescription("Updates an existing post with the provided data")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(
        [FromRoute] int postId,
        [FromBody] PostUpdateRequestDto postUpdateRequestDto
    )
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [Route("{postId}/vote")]
    [EndpointSummary("Vote on a post")]
    [EndpointDescription("A user votes on a post, either upvote or downvote")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VotePost(
        [FromRoute] int postId,
        [FromBody] PostVoteRequestDto postVoteRequestDto
    )
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete]
    [Route("{postId}")]
    [EndpointSummary("Delete a post")]
    [EndpointDescription("Deletes a post by its ID")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost([FromRoute] int postId)
    {
        throw new NotImplementedException();
    }*/
}
