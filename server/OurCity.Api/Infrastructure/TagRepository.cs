using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetTagsByIds(List<Guid> tagIds);
}

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _appDbContext;

    public TagRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Tag>> GetTagsByIds(List<Guid> tagIds)
    {
        return await _appDbContext
            .Tags.Where(t => tagIds.Contains(t.id))
            .ToListAsync();
    }
}
