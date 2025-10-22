/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Contracts.v1.Dtos;

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

    [HttpGet]
    [Route("{id:int}")]
    [EndpointSummary("Get user by ID")]
    [EndpointDescription("Gets a user with the specified ID")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        int number = new Random().Next(0, 2); // 0 inclusive, 2 exclusive
        
        //user does not exist
        if (number == 0)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound);
        }
        
        //was successful
        return Ok(new UserDto
        {
            Id = 1,
            Username = "",
            DisplayName = "",
            IsDeleted = false
        });
    }
    
    [HttpGet]
    [Route("{username}")]
    [EndpointSummary("Get user by username")]
    [EndpointDescription("Gets a user with the specified username")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserByUsername([FromRoute] string username)
    {
        int number = new Random().Next(0, 2); // 0 inclusive, 2 exclusive
        
        //user does not exist
        if (number == 0)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound);
        }
        
        //was successful
        return Ok(new UserDto
        {
            Id = 1,
            Username = "",
            DisplayName = "",
            IsDeleted = false
        });
    }
}