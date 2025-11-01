using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;

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
}