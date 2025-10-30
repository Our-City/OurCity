using Microsoft.EntityFrameworkCore;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Infrastructure;

public interface IPostVoteRepository
{
    Task<PostVote?> GetVoteByPostAndUserId(Guid postId, Guid voterId);
    Task Add(PostVote vote);
    Task Remove(PostVote vote);
}

public class PostVoteRepository : IPostVoteRepository
{
    private readonly AppDbContext _appDbContext;

    public PostVoteRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<PostVote?> GetVoteByPostAndUserId(Guid postId, Guid voterId)
    {
        return await _appDbContext.PostVotes.FirstOrDefaultAsync(v =>
            v.PostId == postId && v.VoterId == voterId
        );
    }

    public async Task Add(PostVote vote)
    {
        await _appDbContext.PostVotes.AddAsync(vote);
    }

    public Task Remove(PostVote vote)
    {
        _appDbContext.PostVotes.Remove(vote);
        return Task.CompletedTask;
    }
}
