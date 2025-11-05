using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Controllers;

/// <credits>
/// Code based off of ChatGPT request for integrating with ASP NET Core Identity
/// </credits>
[ApiController]
[Route("apis/v1/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthenticationController(
        ILogger<AuthenticationController> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager
    )
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    [Route("login")]
    [EndpointSummary("Login")]
    [EndpointDescription("Login to the OurCity application")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Login([FromBody] UserCreateRequestDto createUserRequest)
    {
        var user = await _userManager.FindByNameAsync(createUserRequest.Username);

        if (user == null)
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "Invalid credentials"
            );

        var loginResult = await _signInManager.PasswordSignInAsync(
            user,
            createUserRequest.Password,
            true,
            false
        );

        return loginResult.Succeeded
            ? NoContent()
            : Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Invalid credentials");
    }

    [HttpPost]
    [Authorize]
    [Route("logout")]
    [EndpointSummary("Logout")]
    [EndpointDescription("Logout of the OurCity application")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet]
    [Authorize]
    [Route("me")]
    [EndpointSummary("Me")]
    [EndpointDescription("Get the information of the current user")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        return Ok(
            new
            {
                Id = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value,
                Roles = HttpContext
                    .User.FindAll(ClaimTypes.Role)
                    .Select(role => role.Value)
                    .ToList(),
            }
        );
    }
}
