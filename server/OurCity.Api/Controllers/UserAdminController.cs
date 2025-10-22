/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.
using Microsoft.AspNetCore.Mvc;

namespace OurCity.Api.Controllers;

/// <summary>
/// Strictly admin endpoints in here (some endpoints in the regular controller may still be accessible)
/// </summary>

[ApiController]
[Route("admin/users")]
public class UserAdminController : ControllerBase
{
    private readonly ILogger<UserAdminController> _logger;

    public UserAdminController(ILogger<UserAdminController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    //Authorize by admins?
    [EndpointSummary("Create a new user")]
    [EndpointDescription("Creates a new user with the provided data")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAdminUser(
        [FromBody] UserCreateRequestDto userCreateRequestDto
    )
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    //Authorized by admin
    [Route("Ban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromRoute] int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    //Authorized by admin
    [Route("Ban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ViewReportedUsers([FromRoute] int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    //Authorized by admin
    [Route("Ban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> BanUser([FromRoute] int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    //Authorized by admin
    [Route("Unban/{id}")]
    [EndpointSummary("Ban a user")]
    [EndpointDescription("Ban the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnbanUser([FromRoute] int id)
    {
        throw new NotImplementedException();
    }
}
