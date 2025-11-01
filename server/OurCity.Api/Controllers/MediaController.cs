using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Services;

[ApiController]
[Route("media")]
public class MediaController : ControllerBase
{
    private readonly MediaService _mediaService;

    public MediaController(MediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("{postId}")]
    [ProducesResponseType(typeof(MediaResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> UploadMedia([FromRoute] Guid postId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadMediaAsync(postId, stream, file.FileName);

        return CreatedAtAction(nameof(UploadMedia), new { id = result.Id }, result);
    }

    [HttpGet("post/{postId}")]
    [ProducesResponseType(typeof(List<MediaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMediaForPost([FromRoute] Guid postId)
    {
        var result = await _mediaService.GetMediaForPostAsync(postId);
        return Ok(result);
    }

    [HttpGet("{mediaId:guid}", Name = "GetMediaById")]
    [ProducesResponseType(typeof(MediaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMediaById([FromRoute] Guid mediaId)
    {
        var result = await _mediaService.GetMediaByIdAsync(mediaId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}