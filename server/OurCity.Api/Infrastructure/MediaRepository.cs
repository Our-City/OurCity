using OurCity.Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore; 

public interface IMediaRepository
{
    Task<Media> AddMediaAsync(Media media);
    Task<IEnumerable<Media>> GetMediaByPostIdAsync(Guid postId);
    Task<Media> GetMediaByIdAsync(Guid mediaId);
}

public class MediaRepository : IMediaRepository
{
    private readonly AppDbContext _dbContext;
    public MediaRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<Media> AddMediaAsync(Media media)
    {
        _dbContext.Media.Add(media);
        await _dbContext.SaveChangesAsync();
        return media;
    }

    public async Task<IEnumerable<Media>> GetMediaByPostIdAsync(Guid postId)
    {
        return await _dbContext.Media
            .Where(m => m.PostId == postId)
            .ToListAsync();
    }
    
    public async Task<Media> GetMediaByIdAsync(Guid mediaId)
    {
        return await _dbContext.Media.FindAsync(mediaId);
    }
}