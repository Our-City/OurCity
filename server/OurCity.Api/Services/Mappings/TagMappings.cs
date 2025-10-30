using OurCity.Api.Common.Dtos.Tag;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Mappings;

public static class TagMappings
{
    public static IEnumerable<TagResponseDto> ToDtos(this IEnumerable<Tag> tags)
    {
        return tags.Select(tag => tag.ToDto());
    }

    public static TagResponseDto ToDto(this Tag tag)
    {
        return new TagResponseDto
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}
