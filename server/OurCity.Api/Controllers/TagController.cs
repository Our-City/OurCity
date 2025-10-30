using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Dtos.Tag;
using OurCity.Api.Extensions;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("[controller]s")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly ITagService _tagService;

    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpGet]
    [EndpointSummary("Get all tags")]
    [EndpointDescription("Gets a list of all tags")]
    [ProducesResponseType(typeof(List<TagResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTags()
    {
        var res = await _tagService.GetTags();

        return Ok(res.Data);
    }
}
