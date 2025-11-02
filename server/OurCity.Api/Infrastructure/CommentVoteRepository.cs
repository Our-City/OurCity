using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface ICommentVoteRepository
{
    Task<CommentVote?> GetVoteByCommentAndUserId(Guid commentId, Guid voterId);
    Task Add(CommentVote vote);
    Task Remove(CommentVote vote);
}

public class CommentVoteRepository : ICommentVoteRepository
{
    private readonly AppDbContext _appDbContext;

    public CommentVoteRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<CommentVote?> GetVoteByCommentAndUserId(Guid commentId, Guid voterId)
    {
        return await _appDbContext.CommentVotes.FirstOrDefaultAsync(v =>
            v.CommentId == commentId && v.VoterId == voterId
        );
    }

    public async Task Add(CommentVote vote)
    {
        await _appDbContext.CommentVotes.AddAsync(vote);
    }

    public Task Remove(CommentVote vote)
    {
        _appDbContext.CommentVotes.Remove(vote);
        return Task.CompletedTask;
    }
}
