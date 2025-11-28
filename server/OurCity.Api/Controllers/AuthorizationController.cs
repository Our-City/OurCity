using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Services.Authorization;

namespace OurCity.Api.Controllers;

/// <summary>
/// AuthorizationController has endpoints that lets end users check what policies they have for OurCity.
/// Resource policies are not found in this controller, they should be included in the resource itself.
/// </summary>
[ApiController]
[Route("apis/v1/authorization")]
public class AuthorizationController : ControllerBase
{
    private readonly ILogger<AuthorizationController> _logger;
    private readonly IPolicyService _policyService;

    public AuthorizationController(
        ILogger<AuthorizationController> logger,
        IPolicyService policyService
    )
    {
        _logger = logger;
        _policyService = policyService;
    }

    [HttpGet]
    [Route("can-participate-in-forum")]
    [EndpointSummary("CanParticipateInForum")]
    [EndpointDescription("Check if the current user is authorized to participate in the forum")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanParticipateInForum()
    {
        var isAllowed = await _policyService.CanParticipateInForum();

        return Ok(isAllowed);
    }
}
