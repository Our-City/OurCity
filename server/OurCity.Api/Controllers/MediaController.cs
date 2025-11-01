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
}