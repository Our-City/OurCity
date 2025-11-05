/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("apis/v1/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [EndpointSummary("Create a new user")]
    [EndpointDescription("Creates a new user with the provided data")]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser(
        [FromBody] UserCreateRequestDto userCreateRequestDto
    )
    {
        var createUserResult = await _userService.CreateUser(userCreateRequestDto);

        return createUserResult.IsSuccess
            ? Ok(createUserResult.Data)
            : Problem(statusCode: StatusCodes.Status400BadRequest, detail: createUserResult.Error);
    }

    [HttpPut]
    [Route("{id}")]
    [EndpointSummary("Update an existing user")]
    [EndpointDescription("Updates the user with the specified ID")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UserUpdateRequestDto userUpdateRequestDto
    )
    {
        var user = await _userService.UpdateUser(id, userUpdateRequestDto);
        if (!user.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: user.Error);
        }
        return Ok(user.Data);
    }

    [HttpDelete]
    [Route("{id}")]
    [EndpointSummary("Delete a user")]
    [EndpointDescription("Deletes the user with the specified ID")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var user = await _userService.DeleteUser(id);
        if (!user.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: user.Error);
        }
        return Ok(user.Data);
    }
}
