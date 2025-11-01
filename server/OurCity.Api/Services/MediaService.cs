using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Mappings;

public class MediaService
{
    private readonly AwsS3Service _s3Service;
    private readonly IMediaRepository _mediaRepository;

    public MediaService(AwsS3Service s3Service, IMediaRepository mediaRepository)
    {
        _s3Service = s3Service;
        _mediaRepository = mediaRepository;
    }

    public async Task<MediaResponseDto> UploadMediaAsync(Guid postId, Stream fileStream, string fileName)
    {
        var url = await _s3Service.UploadFileAsync(fileStream, fileName);

        var media = new Media
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            Url = url,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var savedMedia = await _mediaRepository.AddMediaAsync(media);

        return new MediaResponseDto
        {
            Id = savedMedia.Id,
            PostId = savedMedia.PostId,
            Url = savedMedia.Url,
            CreatedAt = savedMedia.CreatedAt,
            UpdatedAt = savedMedia.UpdatedAt
        };
    }

    public async Task<IEnumerable<MediaResponseDto>> GetMediaForPostAsync(Guid postId)
    {
        var media = await _mediaRepository.GetMediaByPostIdAsync(postId);
        return media.ToDtos();
    }

    public async Task<MediaResponseDto> GetMediaByIdAsync(Guid mediaId)
    {
        var media = await _mediaRepository.GetMediaByIdAsync(mediaId);

        return media?.ToDto();
    }

    public async Task<bool> DeleteMediaAsync(Guid mediaId)
    {
        // 1. Finding the media record in the database.
        var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
        if (media == null)
        {
            return false; // Not found
        }

        // 2. Extracting the S3 object key from the URL.
        // The key is the part of the URL after the bucket name and ".com/".
        var uri = new Uri(media.Url);
        var key = uri.AbsolutePath.TrimStart('/');

        // 3. Deleting the file from S3.
        await _s3Service.DeleteFileAsync(key);

        // 4. Deleting the record from the database.
        await _mediaRepository.DeleteMediaAsync(media);

        return true; // Success
    }
}