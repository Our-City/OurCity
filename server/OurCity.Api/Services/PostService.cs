////Supported by generative AI - Claude
/// Prompt: I have a logic for voting, where we receive a vote, either upvote, or downvote.:
/// if vote type = upvote:
/// target list = upvoteUserIdList
/// opposite list = downvoteUserIdList
/// if user id in target list:
/// remove user id from target list
/// if user id is NOT in target list:
/// add user id to target list
/// attempt to remove user id from the opposite list (nothing happens if it's not in this list)
/// help me make this logic more efficient in C#
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IPostService
{
    Task<Result<IEnumerable<PostResponseDto>>> GetPosts();
    Task<Result<PostResponseDto>> GetPostById(int postId);
    Task<Result<PostUpvoteResponseDto>> GetUserUpvoteStatus(int postId, int userId);
    Task<Result<PostDownvoteResponseDto>> GetUserDownvoteStatus(int postId, int userId);
    Task<Result<PostResponseDto>> CreatePost(PostCreateRequestDto postRequestDto);
    Task<Result<PostResponseDto>> UpdatePost(int postId, PostUpdateRequestDto postRequestDto);
    Task<Result<PostResponseDto>> VotePost(int postId, int userId, VoteType voteType);
    Task<Result<PostResponseDto>> DeletePost(int postId);
}

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;

    public PostService(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Result<IEnumerable<PostResponseDto>>> GetPosts()
    {
        var posts = await _postRepository.GetAllPosts();
        return Result<IEnumerable<PostResponseDto>>.Success(posts.ToDtos());
    }

    public async Task<Result<PostResponseDto>> GetPostById(int postId)
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Post does not exist");
        }

        return Result<PostResponseDto>.Success(post.ToDto());
    }

    public async Task<Result<PostUpvoteResponseDto>> GetUserUpvoteStatus(int postId, int userId)
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
        {
            return Result<PostUpvoteResponseDto>.Failure("Post does not exist");
        }

        var isUpvoted = post.UpvotedUserIds.Contains(userId);
        return Result<PostUpvoteResponseDto>.Success(
            new PostUpvoteResponseDto
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                Upvoted = isUpvoted,
            }
        );
    }

    public async Task<Result<PostDownvoteResponseDto>> GetUserDownvoteStatus(int postId, int userId)
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
        {
            return Result<PostDownvoteResponseDto>.Failure("Post does not exist");
        }

        var isDownvoted = post.DownvotedUserIds.Contains(userId);
        return Result<PostDownvoteResponseDto>.Success(
            new PostDownvoteResponseDto
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                Downvoted = isDownvoted,
            }
        );
    }

    public async Task<Result<PostResponseDto>> CreatePost(PostCreateRequestDto postCreateRequestDto)
    {
        var createdPost = await _postRepository.CreatePost(
            postCreateRequestDto.CreateDtoToEntity()
        );
        return Result<PostResponseDto>.Success(createdPost.ToDto());
    }

    public async Task<Result<PostResponseDto>> UpdatePost(
        int postId,
        PostUpdateRequestDto postUpdateRequestDto
    )
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Post does not exist");
        }

        var updatedPost = await _postRepository.UpdatePost(
            postUpdateRequestDto.UpdateDtoToEntity(post)
        );
        return Result<PostResponseDto>.Success(updatedPost.ToDto());
    }

    public async Task<Result<PostResponseDto>> VotePost(int postId, int userId, VoteType voteType)
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Post does not exist");
        }

        var targetList =
            (voteType == VoteType.Upvote) ? post.UpvotedUserIds : post.DownvotedUserIds;
        var oppositeList =
            (voteType == VoteType.Upvote) ? post.DownvotedUserIds : post.UpvotedUserIds;

        if (!targetList.Remove(userId))
        {
            targetList.Add(userId);
            oppositeList.Remove(userId);
        }

        post.UpdatedAt = DateTime.UtcNow;

        var updatedPost = await _postRepository.UpdatePost(post);
        return Result<PostResponseDto>.Success(updatedPost.ToDto());
    }

    public async Task<Result<PostResponseDto>> DeletePost(int postId)
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Post does not exist");
        }

        await _postRepository.DeletePost(post);
        return Result<PostResponseDto>.Success(post.ToDto());
    }
}
