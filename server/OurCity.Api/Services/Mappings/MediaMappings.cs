using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Mappings;

public static class MediaMappings
{
    public static MediaResponseDto ToDto(this Media media)
    {
        return new MediaResponseDto
        {
            Id = media.Id,
            PostId = media.PostId,
            Url = media.Url,
            CreatedAt = media.CreatedAt,
            UpdatedAt = media.UpdatedAt
        };
    }

    public static IEnumerable<MediaResponseDto> ToDtos(this IEnumerable<Media> media)
    {
        return media.Select(m => m.ToDto());
    }
}