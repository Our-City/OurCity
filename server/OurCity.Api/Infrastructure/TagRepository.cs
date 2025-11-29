using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Infrastructure.Database.App;

namespace OurCity.Api.Infrastructure;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllTags();
    Task<IEnumerable<Tag>> GetTagsByIds(List<Guid> tags);
}

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _appDbContext;

    public TagRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        return await _appDbContext.Tags.ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetTagsByIds(List<Guid> tags)
    {
        return await _appDbContext.Tags.Where(t => tags.Contains(t.Id)).ToListAsync();
    }
}
