/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot in creating the GetAllPosts function to fix errors with the infinite
/// scrolling
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
            .Include(p => p.Author!)
            .ThenInclude(a => a.ReceivedReports)
            .Include(p => p.Bookmarks);

        if (postGetAllRequest.SearchTerm is not null)
            query = query.Where(p =>
                p.Title.ToLower().Contains(postGetAllRequest.SearchTerm.ToLower())
                || p.Description.ToLower().Contains(postGetAllRequest.SearchTerm.ToLower())
            );

        if (postGetAllRequest.Tags is not null)
            query = query.Where(p =>
                p.Tags.Select(t => t.Id).Intersect(postGetAllRequest.Tags).Any()
            );

        var sortBy = postGetAllRequest.SortBy?.ToLower();
        var sortOrder = postGetAllRequest.SortOrder ?? SortOrder.Desc;

        if (cursor.HasValue)
        {
            var cursorPost = await _appDbContext
                .Posts.Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == cursor.Value);

            if (cursorPost != null)
            {
                if (sortBy == "votes")
                {
                    var cursorVotes =
                        cursorPost.Votes.Count(v => v.VoteType == VoteType.Upvote)
                        - cursorPost.Votes.Count(v => v.VoteType == VoteType.Downvote);
                    var cursorId = cursorPost.Id;

                    query =
                        sortOrder == SortOrder.Desc
                            ? query.Where(p =>
                                (
                                    p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                                    - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
                                ) < cursorVotes
                                || (
                                    (
                                        p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                                        - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
                                    ) == cursorVotes
                                    && p.Id.CompareTo(cursorId) < 0
                                )
                            )
                            : query.Where(p =>
                                (
                                    p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                                    - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
                                ) > cursorVotes
                                || (
                                    (
                                        p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                                        - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
                                    ) == cursorVotes
                                    && p.Id.CompareTo(cursorId) > 0
                                )
                            );
                }
                else
                {
                    var cursorDate = cursorPost.CreatedAt;
                    var cursorId = cursorPost.Id;

                    query =
                        sortOrder == SortOrder.Desc
                            ? query.Where(p =>
                                p.CreatedAt < cursorDate
                                || (p.CreatedAt == cursorDate && p.Id.CompareTo(cursorId) < 0)
                            )
                            : query.Where(p =>
                                p.CreatedAt > cursorDate
                                || (p.CreatedAt == cursorDate && p.Id.CompareTo(cursorId) > 0)
                            );
                }
            }
        }

        query = (sortBy, sortOrder) switch
        {
            ("votes", SortOrder.Asc) => query
                .OrderBy(p =>
                    p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                    - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
                )
                .ThenBy(p => p.Id),
            ("date", SortOrder.Asc) => query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id),
            ("votes", SortOrder.Desc) => query
                .OrderByDescending(p =>
                    p.Votes.Count(v => v.VoteType == VoteType.Upvote)
                    - p.Votes.Count(v => v.VoteType == VoteType.Downvote)
                )
                .ThenByDescending(p => p.Id),
            ("date", SortOrder.Desc) => query
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.Id),
            _ => query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id),
        };

        return await query.Take(limit).ToListAsync();
    }

    public async Task<Post?> GetFatPostById(Guid postId)
    {
        return await _appDbContext
            .Posts.Include(p => p.Votes)
            .Include(p => p.Comments)
            .Include(p => p.Tags)
            .Include(p => p.Author!)
            .ThenInclude(a => a.ReceivedReports)
            .Include(p => p.Bookmarks)
            .FirstOrDefaultAsync(p => p.Id == postId);
    }

    public async Task<Post?> GetSlimPostbyId(Guid postId)
    {
        return await _appDbContext
            .Posts.Include(p => p.Votes)
            .Include(p => p.Bookmarks)
            .Include(p => p.Author!)
            .ThenInclude(a => a.ReceivedReports)
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
