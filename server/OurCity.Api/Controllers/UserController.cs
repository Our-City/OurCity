/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("[controller]s")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [EndpointSummary("Get all users")]
    [EndpointDescription("Gets a list of all users")]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsers();

        return Ok(users);
    }

    [HttpGet]
    [Route("{id}")]
    [EndpointSummary("Get user by ID")]
    [EndpointDescription("Gets a user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById([FromRoute] int id)
    {
        var user = await _userService.GetUserById(id);
        if (!user.IsSuccess)
        {
            return NotFound(user.Error);
        }
        return Ok(user.Data);
    }

    [HttpPost]
    [EndpointSummary("Create a new user")]
    [EndpointDescription("Creates a new user with the provided data")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser(
        [FromBody] UserCreateRequestDto userCreateRequestDto
    )
    {
        var user = await _userService.CreateUser(userCreateRequestDto);

        return CreatedAtAction(nameof(GetUsers), new { id = user.Data?.Id }, user.Data);
    }

    [HttpPut]
    [Route("{id}")]
    [EndpointSummary("Update an existing user")]
    [EndpointDescription("Updates the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] int id,
        [FromBody] UserUpdateRequestDto userUpdateRequestDto
    )
    {
        var user = await _userService.UpdateUser(id, userUpdateRequestDto);
        if (!user.IsSuccess)
        {
            return NotFound(user.Error);
        }
        return Ok(user.Data);
    }

    [HttpDelete]
    [Route("{id}")]
    [EndpointSummary("Delete a user")]
    [EndpointDescription("Deletes the user with the specified ID")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        var user = await _userService.DeleteUser(id);
        if (!user.IsSuccess)
        {
            return NotFound(user.Error);
        }
        return Ok(user.Data);
    }
}
