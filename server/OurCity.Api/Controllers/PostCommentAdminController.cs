/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API development

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("admin/posts/{postId}/comments")]
[Authorize]
public class PostCommentAdminController : ControllerBase
{
    private readonly ILogger<PostCommentAdminController> _logger;

    public PostCommentAdminController(ILogger<PostCommentAdminController> logger)
    {
        _logger = logger;
    }

    [HttpPut("{commentId}")]
    [EndpointSummary("Delete a comment")]
    [EndpointDescription("Deletes a comment associated with a specific post")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> HideComment(
        [FromRoute] int postId,
        [FromRoute] int commentId
    )
    {
        int number = new Random().Next(0, 3); // 0 inclusive, 3 exclusive
        
        //user is not an admin
        if (number == 0)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized);
        }
        
        //comment doesnt even exist
        if (number == 1)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound);
        }
        
        //was successful
        return NoContent();
    }
}
