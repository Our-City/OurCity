using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Services.Authorization;

namespace OurCity.Api.Controllers;

/// <summary>
/// AuthorizationController has endpoints that lets end users check what policies they have for OurCity
/// </summary>
[ApiController]
[Route("authorization")]
public class AuthorizationController : ControllerBase
{
    private readonly ILogger<AuthorizationController> _logger;

    public AuthorizationController(
        ILogger<AuthorizationController> logger
    )
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("CanCreatePosts")]
    [EndpointSummary("CanCreatePosts")]
    [EndpointDescription("Check if the current user is authorized to create posts")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanCreatePosts()
    {
        return Ok(new CanDoPolicy
        {
            Authorized = true
        });
    }

    [HttpGet]
    [Route("CanMutateThisPost/{postId}")]
    [EndpointSummary("CanMutateThisPost")]
    [EndpointDescription("Check if the current user is authorized to mutate a given post")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> CanMutateThisPost([FromRoute] int postId)
    {
        return Ok(new CanDoPolicy
        {
            Authorized = true
        });
    }
}

public class CanDoPolicy
{
    public bool Authorized { get; set; } = false;
}