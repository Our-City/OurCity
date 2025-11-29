/// Generative AI - CoPilot was used to assist in the creation of this file.
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Infrastructure.Database.App;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IMediaService
{
    Task<Result<MediaResponseDto>> UploadMediaAsync(
        Guid userId,
        Guid postId,
        Stream fileStream,
        string fileName
    );
    Task<Result<IEnumerable<MediaResponseDto>>> GetMediaForPostAsync(Guid postId);
    Task<Result<MediaResponseDto>> GetMediaByIdAsync(Guid mediaId);
    Task<Result<MediaResponseDto>> DeleteMediaAsync(Guid userId, Guid mediaId);
}

public class MediaService : IMediaService
{
    private readonly AwsS3Service _s3Service;
    private readonly IMediaRepository _mediaRepository;
    private readonly IPostRepository _postRepository;

    public MediaService(
        AwsS3Service s3Service,
        IMediaRepository mediaRepository,
        IPostRepository postRepository
    )
    {
        _s3Service = s3Service;
        _mediaRepository = mediaRepository;
        _postRepository = postRepository;
    }

    public async Task<Result<MediaResponseDto>> UploadMediaAsync(
        Guid userId,
        Guid postId,
        Stream fileStream,
        string fileName
    )
    {
        var post = await _postRepository.GetSlimPostbyId(postId);

        if (post == null)
        {
            return Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound);
        }

        if (userId != post.AuthorId)
        {
            return Result<MediaResponseDto>.Failure(ErrorMessages.MediaUnauthorized);
        }

        var url = await _s3Service.UploadFileAsync(fileStream, fileName);

        var media = new Media
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            Url = url,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var savedMedia = await _mediaRepository.AddMediaAsync(media);

        return Result<MediaResponseDto>.Success(savedMedia.ToDto());
    }

    public async Task<Result<IEnumerable<MediaResponseDto>>> GetMediaForPostAsync(Guid postId)
    {
        var media = await _mediaRepository.GetMediaByPostIdAsync(postId);
        return Result<IEnumerable<MediaResponseDto>>.Success(media.ToDtos());
    }

    public async Task<Result<MediaResponseDto>> GetMediaByIdAsync(Guid mediaId)
    {
        var media = await _mediaRepository.GetMediaByIdAsync(mediaId);

        if (media == null)
        {
            return Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound);
        }

        return Result<MediaResponseDto>.Success(media.ToDto());
    }

    public async Task<Result<MediaResponseDto>> DeleteMediaAsync(Guid userId, Guid mediaId)
    {
        // 1. Finding the media record in the database.
        var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
        if (media == null)
        {
            return Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound); // Not found
        }

        var post = await _postRepository.GetSlimPostbyId(media.PostId);

        if (post == null)
        {
            return Result<MediaResponseDto>.Failure(ErrorMessages.MediaNotFound);
        }

        if (userId != post.AuthorId)
        {
            return Result<MediaResponseDto>.Failure(ErrorMessages.MediaUnauthorized);
            ;
        }

        // 2. Extracting the S3 object key from the URL.
        // The key is the part of the URL after the bucket name and ".com/".
        var uri = new Uri(media.Url);
        var key = uri.AbsolutePath.TrimStart('/');

        // 3. Deleting the file from S3.
        await _s3Service.DeleteFileAsync(key);

        // 4. Deleting the record from the database.
        await _mediaRepository.DeleteMediaAsync(media);

        return Result<MediaResponseDto>.Success(media.ToDto()); // Success
    }
}
