/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Contracts.v1.Dtos;

namespace OurCity.Api.Controllers;

/// <summary>
/// Strictly admin endpoints in here (some endpoints in the regular controller may still be accessible)
/// </summary>

[ApiController]
[Route("admin/users")]
[Authorize]
public class UserAdminController : ControllerBase
{
    private readonly ILogger<UserAdminController> _logger;

    public UserAdminController(ILogger<UserAdminController> logger)
    {
        _logger = logger;
    }

    /*
    /// <summary>
    /// note: had a createadminuser.. but thinking about my WTs i feel like they usually just make an account and u have to switch your password lollll.
    /// </summary>
    [HttpPost]
    //Authorize by admins?
    [EndpointSummary("Create a new user")]
    [EndpointDescription("Creates a new user with the provided data")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAdminUser(
        [FromBody] UserCreateRequestDto userCreateRequestDto
    )
    {
        throw new NotImplementedException();
    }*/

    [HttpPost]
    [Route("Ban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ViewReportedUsers([FromRoute] int id)
    {
        return Ok(new List<UserDto>());
    }
    
    [HttpPost]
    [Route("Ban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> BanUser([FromRoute] int id)
    {
        int number = new Random().Next(0, 3); // 0 inclusive, 3 exclusive
        
        //user is not an admin
        if (number == 0)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized);
        }
        
        //user doesnt even exist
        if (number == 1)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound);
        }
        
        //was successful
        return NoContent();
    }
    
    [HttpPost]
    //Authorized by admin
    [Route("Unban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UnbanUser([FromRoute] int id)
    {
        int number = new Random().Next(0, 3); // 0 inclusive, 3 exclusive
        
        //user is not an admin
        if (number == 0)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized);
        }
        
        //user doesnt even exist
        if (number == 1)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound);
        }
        
        //was successful
        return NoContent();
    }
}
