using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("admin/posts")]
public class PostAdminController : ControllerBase
{
    private readonly ILogger<PostAdminController> _logger;

    public PostAdminController(ILogger<PostAdminController> logger)
    {
        _logger = logger;
    }

    [HttpPut]
    [Route("{postId}")]
    [EndpointSummary("Delete a post")]
    [EndpointDescription("Deletes a post by its ID")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> HidePost([FromRoute] int postId)
    {
        int number = new Random().Next(0, 3); // 0 inclusive, 3 exclusive
        
        //user is not an admin
        if (number == 0)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized);
        }
        
        //post doesnt even exist
        if (number == 1)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound);
        }
        
        //was successful
        return NoContent();
    }
}
