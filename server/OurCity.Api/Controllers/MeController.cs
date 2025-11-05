using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Extensions;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Authorize]
[Route("/apis/v1/me")]
public class MeController : ControllerBase
{
    private readonly ILogger<MeController> _logger;
    private readonly IUserService _userService;

    public MeController(IUserService userService, ILogger<MeController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [EndpointSummary("Get my info")]
    [EndpointDescription("Get my OurCity information")]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe()
    {
        var myId = HttpContext.User.GetUserId();

        if (!myId.HasValue)
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "You must be logged in."
            );

        var getResult = await _userService.GetUserById(myId.Value);

        if (!getResult.IsSuccess)
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: getResult.Error);

        return Ok(getResult.Data);
    }

    [HttpPut]
    [EndpointSummary("Update my account")]
    [EndpointDescription("Update my OurCity account")]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMe([FromBody] UserUpdateRequestDto userUpdateRequestDto)
    {
        var myId = HttpContext.User.GetUserId();

        if (!myId.HasValue)
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "You must be logged in."
            );

        var updateResult = await _userService.UpdateUser(myId.Value, userUpdateRequestDto);

        if (!updateResult.IsSuccess)
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: updateResult.Error);

        return Ok(updateResult.Data);
    }

    [HttpDelete]
    [EndpointSummary("Delete my account")]
    [EndpointDescription("Delete my OurCity account")]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMe()
    {
        var myId = HttpContext.User.GetUserId();

        if (!myId.HasValue)
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "You must be logged in."
            );

        var deleteResult = await _userService.DeleteUser(myId.Value);

        if (!deleteResult.IsSuccess)
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: deleteResult.Error);

        return Ok(deleteResult.Data);
    }
}
