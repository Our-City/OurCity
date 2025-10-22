/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.
using Microsoft.AspNetCore.Mvc;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    class GetUsersRequest
    {
        //
    }

    class GetUsersResponse
    {
        
    }

    class UserResponseDto
    {
        
    }
    
    [HttpGet]
    [Route("me")]
    [EndpointSummary("Get my user")]
    [EndpointDescription("Get ma user")]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe()
    {
        throw new NotImplementedException();
    }
    
    [HttpPut]
    [Route("me")]
    [EndpointSummary("Get my user")]
    [EndpointDescription("Get ma user")]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
    //[ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMe()
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete]
    //Authorized by admin
    [Route("{id}")]
    [EndpointSummary("Delete a user")]
    [EndpointDescription("Deletes the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMe([FromRoute] int id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}")]
    [EndpointSummary("Get user by ID")]
    [EndpointDescription("Gets a user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    [Route("{id}")]
    [EndpointSummary("Get user by ID")]
    [EndpointDescription("Gets a user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserByUsername([FromRoute] int id)
    {
        throw new NotImplementedException();
    }
}
