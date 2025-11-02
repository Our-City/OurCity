/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard repository functions
/// based on common patterns in C# for repository implementations
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetCommentsByPostId(int postId);
    Task<Comment?> GetCommentById(int postId, int commentId);
    Task<Comment> CreateComment(Comment comment);
    Task<Comment> UpdateComment(Comment comment);
    Task<Comment> DeleteComment(Comment comment);
}

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _appDbContext;

    public CommentRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId)
    {
        return await _appDbContext
            .Comments.Where(c => c.PostId == postId && !c.IsDeleted)
            .ToListAsync();
    }

    public async Task<Comment?> GetCommentById(int postId, int commentId)
    {
        return await _appDbContext
            .Comments.Where(c => c.PostId == postId && c.Id == commentId && !c.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<Comment> CreateComment(Comment comment)
    {
        _appDbContext.Comments.Add(comment);
        await _appDbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment> UpdateComment(Comment comment)
    {
        _appDbContext.Comments.Update(comment);
        await _appDbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment> DeleteComment(Comment comment)
    {
        // soft deletion in db  (mark Comment as deleted)
        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;
        _appDbContext.Comments.Update(comment);
        await _appDbContext.SaveChangesAsync();
        return comment;
    }
}
