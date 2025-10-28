using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllPosts();
    Task<Post?> GetPostById(Guid postId);
    Task<Post> CreatePost(Post post);
    Task<Post> UpdatePost(Post post);
    Task<Post> DeletePost(Post post);
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
            .Posts.Include(p => p.Media)
            .Include(p => p.Comments)
            .ToListAsync();
    }

    public async Task<Post?> GetPostById(Guid postId)
    {
        return await _appDbContext
            .Posts.Include(p => p.Media)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postId);
    }

    public async Task<Post> CreatePost(Post post)
    {
        _appDbContext.Posts.Add(post);
        await _appDbContext.SaveChangesAsync();
        return post;
    }

    public async Task<Post> UpdatePost(Post post)
    {
        _appDbContext.Posts.Update(post);
        await _appDbContext.SaveChangesAsync();
        return post;
    }

    public async Task<Post> DeletePost(Post post)
    {
        _appDbContext.Posts.Remove(post);
        await _appDbContext.SaveChangesAsync();
        return post;
    }
}
