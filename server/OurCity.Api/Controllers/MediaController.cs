/// Generative AI - CoPilot was used to assist in the creation of this file.
using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Extensions;

[ApiController]
[Route("apis/v1/media")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("{postId}")]
    [ProducesResponseType(typeof(MediaResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> UploadMedia([FromRoute] Guid postId, IFormFile file)
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        if (file == null || file.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "No file uploaded");
        }

        using var stream = file.OpenReadStream();
        var result = await _mediaService.UploadMediaAsync(
            userId.Value,
            postId,
            stream,
            file.FileName
        );

        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: (
                    result.Error != null && result.Error.Equals(ErrorMessages.MediaNotFound)
                )
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: result.Error
            );
        }

        return CreatedAtAction(nameof(UploadMedia), new { id = result.Data?.Id }, result.Data);
    }

    [HttpGet("posts/{postId}")]
    [ProducesResponseType(typeof(List<MediaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMediaForPost([FromRoute] Guid postId)
    {
        var result = await _mediaService.GetMediaForPostAsync(postId);
        return Ok(result.Data);
    }

    [HttpGet("{mediaId:guid}", Name = "GetMediaById")]
    [ProducesResponseType(typeof(MediaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMediaById([FromRoute] Guid mediaId)
    {
        var result = await _mediaService.GetMediaByIdAsync(mediaId);

        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: ErrorMessages.MediaNotFound
            );
        }

        return Ok(result.Data);
    }

    [HttpDelete("{mediaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMedia([FromRoute] Guid mediaId)
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                detail: "User not authenticated"
            );
        }

        var result = await _mediaService.DeleteMediaAsync(userId.Value, mediaId);

        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: (
                    result.Error != null && result.Error.Equals(ErrorMessages.MediaNotFound)
                )
                    ? StatusCodes.Status404NotFound
                    : StatusCodes.Status403Forbidden,
                detail: result.Error
            );
        }

        return NoContent(); // This is a standard response for a successful DELETE
    }
}
