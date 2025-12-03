using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IPostBookmarkRepository
{
    Task<PostBookmark?> GetBookmarkByUserAndPostId(Guid userId, Guid postId);
    Task<IEnumerable<PostBookmark>> GetBookmarksByUser(Guid userId, Guid? cursor, int limit);
    Task Add(PostBookmark bookmark);
    Task Remove(PostBookmark bookmark);
    Task SaveChangesAsync();
}

public class PostBookmarkRepository : IPostBookmarkRepository
{
    private readonly AppDbContext _appDbContext;

    public PostBookmarkRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<PostBookmark?> GetBookmarkByUserAndPostId(Guid userId, Guid postId)
    {
        return await _appDbContext
            .PostBookmarks.Include(b => b.Post)
            .FirstOrDefaultAsync(b => b.PostId == postId && b.UserId == userId);
    }

    public async Task<IEnumerable<PostBookmark>> GetBookmarksByUser(
        Guid userId,
        Guid? cursor,
        int limit
    )
    {
        IQueryable<PostBookmark> query = _appDbContext
            .PostBookmarks.Where(b => b.UserId == userId)
            .Include(b => b.Post)
                .ThenInclude(p => p.Author)
            .OrderByDescending(b => b.BookmarkedAt);

        if (cursor.HasValue)
        {
            var cursorBookmark = await _appDbContext.PostBookmarks.FindAsync(cursor.Value);
            if (cursorBookmark != null)
            {
                query = query.Where(b =>
                    b.BookmarkedAt < cursorBookmark.BookmarkedAt
                    || (
                        b.BookmarkedAt == cursorBookmark.BookmarkedAt
                        && b.Id.CompareTo(cursorBookmark.Id) < 0
                    )
                );
            }
        }

        return await query.Take(limit).ToListAsync();
    }

    public async Task Add(PostBookmark bookmark)
    {
        await _appDbContext.PostBookmarks.AddAsync(bookmark);
    }

    public Task Remove(PostBookmark bookmark)
    {
        _appDbContext.PostBookmarks.Remove(bookmark);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}
