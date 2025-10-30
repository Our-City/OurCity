using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Dtos.Tag;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface ITagService
{
    Task<Result<IEnumerable<TagResponseDto>>> GetTags();
}

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(
        ITagRepository tagRepository
    )
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<IEnumerable<TagResponseDto>>> GetTags()
    {
        var tags = await _tagRepository.GetAllTags();
        return Result<IEnumerable<TagResponseDto>>.Success(tags.ToDtos());
    }
}