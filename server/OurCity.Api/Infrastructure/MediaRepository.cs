using OurCity.Api.Infrastructure.Database;

public interface IMediaRepository
{
    Task<Media> AddMediaAsync(Media media);
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
}