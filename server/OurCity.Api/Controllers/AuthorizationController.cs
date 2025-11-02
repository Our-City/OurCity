using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Services.Authorization;

namespace OurCity.Api.Controllers;

/// <summary>
/// AuthorizationController has endpoints that lets end users check what policies they have for OurCity
/// </summary>
[ApiController]
[Route("[controller]")]
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
    [Route("CanCreatePosts")]
    [EndpointSummary("CanCreatePosts")]
    [EndpointDescription("Check if the current user is authorized to create posts")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanCreatePosts()
    {
        var isAllowed = await _policyService.CheckPolicy(HttpContext.User, Policy.CanCreatePosts);

        return Ok(isAllowed);
    }

    [HttpGet]
    [Route("CanMutateThisPost/{postId}")]
    [EndpointSummary("CanMutateThisPost")]
    [EndpointDescription("Check if the current user is authorized to mutate a given post")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanMutateThisPost([FromRoute] int postId)
    {
        var isAllowed = await _policyService.CheckResourcePolicy(
            HttpContext.User,
            Policy.CanMutateThisPost,
            postId
        );

        return Ok(isAllowed);
    }
}
