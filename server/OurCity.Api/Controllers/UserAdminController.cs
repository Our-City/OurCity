using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Authorize]
[Route("apis/v1/admin/users")]
public class UserAdminController : ControllerBase
{
    private readonly ILogger<UserAdminController> _logger;
    private readonly IUserService _userService;

    public UserAdminController(IUserService userService, ILogger<UserAdminController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    [Route("reported")]
    [EndpointSummary("Get highly reported users")]
    [EndpointDescription("Get users that are highly reported")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(PaginatedResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHighlyReportedUsers()
    {
        var userFilter = new UserFilter { MinimumNumReports = 5 };

        var users = await _userService.GetUsers(userFilter);

        if (!users.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, detail: users.Error);
        }

        return Ok(users.Data);
    }

    [HttpGet]
    [Route("banned")]
    [EndpointSummary("Get banned users")]
    [EndpointDescription("Get users that are banned")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(PaginatedResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBannedUsers()
    {
        var userFilter = new UserFilter { IsBanned = true };

        var users = await _userService.GetUsers(userFilter);

        if (!users.IsSuccess)
        {
            return Problem(statusCode: StatusCodes.Status403Forbidden, detail: users.Error);
        }

        return Ok(users.Data);
    }

    [HttpPut]
    [Route("{username}/ban")]
    [EndpointSummary("Ban user")]
    [EndpointDescription("Ban a user")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> BanUser([FromRoute] string username)
    {
        _logger.LogInformation("BanUser called with username: {Username}", username);
        var res = await _userService.BanUser(username);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.UserNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return Ok(res.Data);
    }

    [HttpPut]
    [Route("{username}/unban")]
    [EndpointSummary("Unban user")]
    [EndpointDescription("Unban a user")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnbanUser([FromRoute] string username)
    {
        _logger.LogInformation("UnbanUser called with username: {Username}", username);
        var res = await _userService.UnbanUser(username);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.UserNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return Ok(res.Data);
    }

    [HttpPut]
    [Route("{username}/promote-to-admin")]
    [EndpointSummary("Promote user to admin")]
    [EndpointDescription("Promote a user to admin")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> PromoteUserToAdmin([FromRoute] string username)
    {
        _logger.LogInformation("PromoteUserToAdmin called with username: {Username}", username);
        var res = await _userService.PromoteUserToAdmin(username);

        if (!res.IsSuccess)
        {
            return Problem(
                statusCode: (res.Error != null && res.Error.Equals(ErrorMessages.UserNotFound))
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: res.Error
            );
        }

        return NoContent();
    }
}
