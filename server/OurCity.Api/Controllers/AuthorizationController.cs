using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Services.Authorization;

namespace OurCity.Api.Controllers;

/// <summary>
/// AuthorizationController has endpoints that lets end users check what policies they have for OurCity
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
    [EndpointDescription("Check if the current user is authorized to participate in the forum (create posts/comments, vote, etc)")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanParticipateInForum()
    {
        var isAllowed = await _policyService.CheckPolicy(HttpContext.User, Policy.CanParticipateInForum);

        return Ok(isAllowed);
    }

    [HttpGet]
    [Route("can-mutate-post/{postId}")]
    [EndpointSummary("CanMutateThisPost")]
    [EndpointDescription("Check if the current user is authorized to mutate a given post")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanMutateThisPost([FromRoute] Guid postId)
    {
        var isAllowed = await _policyService.CheckResourcePolicy(
            HttpContext.User,
            Policy.CanMutateThisPost,
            postId
        );

        return Ok(isAllowed);
    }
}
