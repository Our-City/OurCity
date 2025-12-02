using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllPosts(PostGetAllRequestDto postGetAllRequest);
    Task<Post?> GetFatPostById(Guid postId);
    Task<Post?> GetSlimPostbyId(Guid postId);
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

    public async Task<IEnumerable<Post>> GetAllPosts(PostGetAllRequestDto postGetAllRequest)
    {
        var cursor = postGetAllRequest.Cursor;
        var limit = postGetAllRequest.Limit;

        IQueryable<Post> query = _appDbContext
            .Posts.Where(p => !p.IsDeleted)
            .Where(p => p.Author == null || !p.Author.IsBanned)
            .Where(p => p.Author == null || p.Author.ReceivedReports.Count < 5)
            .Include(p => p.Votes)
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .Include(p => p.Author);

        if (postGetAllRequest.SearchTerm is not null)
            query = query.Where(p =>
                p.Title.ToLower().Contains(postGetAllRequest.SearchTerm.ToLower())
                || p.Description.ToLower().Contains(postGetAllRequest.SearchTerm.ToLower())
            );

        if (postGetAllRequest.Tags is not null)
            query = query.Where(p =>
                p.Tags.Select(t => t.Id).Intersect(postGetAllRequest.Tags).Any()
            );

        query = (postGetAllRequest.SortBy?.ToLower(), postGetAllRequest.SortOrder) switch
        {
            ("votes", SortOrder.Asc) => query.OrderBy(p =>
                p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
            ),
            ("date", SortOrder.Asc) => query.OrderBy(p => p.CreatedAt),
            ("votes", SortOrder.Desc) => query.OrderByDescending(p =>
                p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
            ),
            ("date", SortOrder.Desc) => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id),
        };

        if (cursor.HasValue)
        {
            var cursorPost = await _appDbContext.Posts.FindAsync(cursor.Value);
            if (cursorPost != null)
            {
                query = query.Where(p =>
                    p.CreatedAt < cursorPost.CreatedAt
                    || (p.CreatedAt == cursorPost.CreatedAt && p.Id.CompareTo(cursorPost.Id) < 0)
                );
            }
        }

        return await query.Take(limit).ToListAsync();
    }

    public async Task<Post?> GetFatPostById(Guid postId)
    {
        return await _appDbContext
            .Posts.Include(p => p.Votes)
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == postId);
    }

    public async Task<Post?> GetSlimPostbyId(Guid postId)
    {
        return await _appDbContext
            .Posts.Include(p => p.Votes)
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
