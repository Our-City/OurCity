using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllPosts();
    Task<Post?> GetPostById(Guid postId);
    Task<Post> CreatePost(Post post);
    Task SaveChangesAsync(); 
}

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _appDbContext;

    public PostRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Post>> GetAllPosts()
    {
        return await _appDbContext
            .Posts.Include(p => p.Votes)
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .ToListAsync();
    }

    public async Task<Post?> GetPostById(Guid postId)
    {
        return await _appDbContext
            .Posts.Include(p => p.Votes)
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == postId);
    }

    public async Task<Post> CreatePost(Post post)
    {
        _appDbContext.Posts.Add(post);
        await _appDbContext.SaveChangesAsync();
        return post;
    }

    public async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync(); 
    }
}
