using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

/// <summary>
/// NOTE: THIS IS JUST A FULL STUB FOR THE ACTUAL AUTHENTICATION
/// </summary>
/// <credits>
/// Code taken largely from ChatGPT prompt asking for cookie authentication where it just takes the user submitted name for the identity
/// </credits>
[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IUserService _userService;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        IUserService userService
    )
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost]
    [Route("Login/{username}")]
    [EndpointSummary("Login")]
    [EndpointDescription("Login to the OurCity application")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Login([FromRoute] [Required] [MinLength(1)] string username)
    {
        var claims = new List<Claim> { };

        var getUserResult = await _userService.GetUserByUsername(username);

        if (getUserResult.IsSuccess)
        {
            claims.Add(
                new Claim(ClaimTypes.NameIdentifier, getUserResult.Data?.Id.ToString() ?? "")
            );
            claims.Add(new Claim(ClaimTypes.Name, username));
        }
        else
        {
            var createUserResult = await _userService.CreateUser(
                new UserCreateRequestDto { Username = username }
            );
            var newUser = createUserResult.Data;

            claims.Add(new Claim(ClaimTypes.NameIdentifier, newUser?.Id.ToString() ?? ""));
            claims.Add(new Claim(ClaimTypes.Name, username));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return NoContent();
    }

    [HttpPost]
    [Authorize]
    [Route("Logout")]
    [EndpointSummary("Logout")]
    [EndpointDescription("Logout of the OurCity application")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }

    [HttpGet]
    [Route("Me")]
    [EndpointSummary("Me")]
    [EndpointDescription("Get the information of the current user")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        var isUserAuthenticated = User.Identity?.IsAuthenticated ?? false;

        if (!isUserAuthenticated)
            return Unauthorized();

        return Ok(User.FindFirst(ClaimTypes.Name)?.Value);
    }
}
