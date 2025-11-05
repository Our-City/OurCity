/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard repository functions
/// based on common patterns in C# for repository implementations
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetCommentsForPost(Guid postId, Guid? cursor, int limit);
    Task<Comment?> GetCommentById(Guid commentId);
    Task<Comment> CreateComment(Comment comment);
    Task SaveChangesAsync();
}

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _appDbContext;

    public CommentRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Comment>> GetCommentsForPost(Guid postId, Guid? cursor, int limit)
    {
        IQueryable<Comment> query = _appDbContext
            .Comments.Where(c => c.PostId == postId)
            .Include(c => c.Author)
            .Include(c => c.Votes)
            .OrderByDescending(c => c.CreatedAt)
            .ThenByDescending(c => c.Id);

        if (cursor.HasValue)
        {
            var cursorComment = await _appDbContext.Comments.FindAsync(cursor.Value);
            if (cursorComment != null)
            {
                query = query.Where(c =>
                    c.CreatedAt < cursorComment.CreatedAt
                    || (
                        c.CreatedAt == cursorComment.CreatedAt
                        && c.Id.CompareTo(cursorComment.Id) < 0
                    )
                );
            }
        }

        return await query.Take(limit).ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId)
    {
        return await _appDbContext
            .Comments.Include(c => c.Votes)
            .Where(c => c.PostId == postId)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentById(Guid commentId)
    {
        return await _appDbContext
            .Comments
            .Include(c => c.Votes)
            .Include(c => c.Author)
            .Where(c => c.Id == commentId)
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }

    public async Task<Comment> CreateComment(Comment comment)
    {
        _appDbContext.Comments.Add(comment);
        await _appDbContext.SaveChangesAsync();
        return comment;
    }

    public async Task SaveChangesAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
}
