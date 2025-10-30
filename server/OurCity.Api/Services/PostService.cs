using OurCity.Api.Common;
using OurCity.Api.Common.Dtos;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IPostService
{
    Task<Result<IEnumerable<PostResponseDto>>> GetPosts(Guid? userId);
    Task<Result<PostResponseDto>> GetPostById(Guid? userId, Guid postId);
    Task<Result<PostResponseDto>> CreatePost(Guid userId, PostCreateRequestDto postRequestDto);
    Task<Result<PostResponseDto>> UpdatePost(
        Guid userId,
        Guid postId,
        PostUpdateRequestDto postRequestDto
    );
    Task<Result<PostResponseDto>> VotePost(
        Guid userId,
        Guid postId,
        PostVoteRequestDto postVoteRequestDto
    );
    Task<Result<PostResponseDto>> DeletePost(Guid userId, Guid postId);
}

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IPostVoteRepository _postVoteRepository;

    public PostService(
        IPostRepository postRepository,
        ITagRepository tagRepository,
        IPostVoteRepository postVoteRepository
    )
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _postVoteRepository = postVoteRepository;
    }

    public async Task<Result<IEnumerable<PostResponseDto>>> GetPosts(Guid? userId)
    {
        var posts = await _postRepository.GetAllPosts();
        return Result<IEnumerable<PostResponseDto>>.Success(posts.ToDtos(userId));
    }

    public async Task<Result<PostResponseDto>> GetPostById(Guid? userId, Guid postId)
    {
        var post = await _postRepository.GetFatPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Resource not found");
        }

        return Result<PostResponseDto>.Success(post.ToDto(userId));
    }

    public async Task<Result<PostResponseDto>> CreatePost(
        Guid userId,
        PostCreateRequestDto postCreateRequestDto
    )
    {
        var tags = await _tagRepository.GetTagsByIds(postCreateRequestDto.TagIds);

        var createdPost = await _postRepository.CreatePost(
            postCreateRequestDto.CreateDtoToEntity(userId, tags.ToList())
        );
        return Result<PostResponseDto>.Success(createdPost.ToDto(userId));
    }

    public async Task<Result<PostResponseDto>> UpdatePost(
        Guid userId,
        Guid postId,
        PostUpdateRequestDto postUpdateRequestDto
    )
    {
        var post = await _postRepository.GetFatPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Resource not found");
        }

        if (userId != post.AuthorId)
        {
            return Result<PostResponseDto>.Failure(
                "You do not have permission to perform this action"
            );
        }

        var tags =
            postUpdateRequestDto.TagIds != null
                ? await _tagRepository.GetTagsByIds(postUpdateRequestDto.TagIds)
                : null;

        postUpdateRequestDto.UpdateDtoToEntity(post, tags?.ToList());
        await _postRepository.SaveChangesAsync();

        return Result<PostResponseDto>.Success(post.ToDto(userId));
    }

    public async Task<Result<PostResponseDto>> VotePost(
        Guid userId,
        Guid postId,
        PostVoteRequestDto postVoteRequestDto
    )
    {
        var post = await _postRepository.GetSlimPostbyId(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Resource not found");
        }

        var existingVote = await _postVoteRepository.GetVoteByPostAndUserId(postId, userId);
        var requestedVoteType = postVoteRequestDto.VoteType;

        if (existingVote != null && requestedVoteType == VoteType.NoVote)
        {
            await _postVoteRepository.Remove(existingVote);
        }
        else if (existingVote == null && requestedVoteType != VoteType.NoVote)
        {
            await _postVoteRepository.Add(
                new PostVote
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    VoterId = userId,
                    VoteType = requestedVoteType,
                    VotedAt = DateTime.UtcNow,
                }
            );
        }
        else if (existingVote != null && requestedVoteType != VoteType.NoVote)
        {
            existingVote.VoteType = requestedVoteType;
            existingVote.VotedAt = DateTime.UtcNow;
        }

        post.UpdatedAt = DateTime.UtcNow;
        await _postRepository.SaveChangesAsync();

        return Result<PostResponseDto>.Success(post.ToDto(userId));
    }

    public async Task<Result<PostResponseDto>> DeletePost(Guid userId, Guid postId)
    {
        var post = await _postRepository.GetSlimPostbyId(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure("Resource not found");
        }

        if (userId != post.AuthorId)
        {
            return Result<PostResponseDto>.Failure(
                "You do not have permission to perform this action"
            );
        }

        post.IsDeleted = true;
        post.UpdatedAt = DateTime.UtcNow;
        await _postRepository.SaveChangesAsync();

        return Result<PostResponseDto>.Success(post.ToDto(userId));
    }
}
