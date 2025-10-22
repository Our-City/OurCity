/// Generative AI - CoPilot was used to assist in the creation of this file.
///  CoPilot assisted by generating boilerplate code for standard CRUD operations
///  and routing attributes based on common patterns in ASP.NET API developmeent.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Contracts.v1.Dtos;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("me")]
public class MeController : ControllerBase
{
    private readonly ILogger<MeController> _logger;

    public MeController(ILogger<MeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    [EndpointSummary("Get my user")]
    [EndpointDescription("Get ma user")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe()
    {
        //was successful
        return Ok(new UserDto
        {
            Id = 1,
            Username = "",
            DisplayName = "",
            IsDeleted = false
        });
    }
    
    [HttpPut]
    [Authorize]
    [EndpointSummary("Update my user")]
    [EndpointDescription("Update ma user")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMe()
    {
        //was successful
        return Ok(new UserDto
        {
            Id = 1,
            Username = "",
            DisplayName = "",
            IsDeleted = false
        });
    }
    
    [HttpDelete]
    [Authorize]
    [EndpointSummary("Delete my user")]
    [EndpointDescription("Deletes ma user")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMe()
    {
        return NoContent();
    }
}