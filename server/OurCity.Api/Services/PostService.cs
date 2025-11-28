using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Authorization;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IPostService
{
    Task<Result<PaginatedResponseDto<PostResponseDto>>> GetPosts(
        PostGetAllRequestDto postGetAllRequestDto
    );
    Task<Result<PostResponseDto>> GetPostById(Guid postId);
    Task<Result<PostResponseDto>> CreatePost(PostCreateRequestDto postRequestDto);
    Task<Result<PostResponseDto>> UpdatePost(Guid postId, PostUpdateRequestDto postRequestDto);
    Task<Result<PostResponseDto>> VotePost(Guid postId, PostVoteRequestDto postVoteRequestDto);
    Task<Result<PostResponseDto>> DeletePost(Guid postId);
}

public class PostService : IPostService
{
    private readonly ICurrentUser _requestingUser;
    private readonly IPolicyService _policyService;
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IPostVoteRepository _postVoteRepository;

    public PostService(
        ICurrentUser requestingUser,
        IPolicyService policyService,
        IPostRepository postRepository,
        ITagRepository tagRepository,
        IPostVoteRepository postVoteRepository
    )
    {
        _requestingUser = requestingUser;
        _policyService = policyService;
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _postVoteRepository = postVoteRepository;
    }

    public async Task<Result<PaginatedResponseDto<PostResponseDto>>> GetPosts(
        PostGetAllRequestDto postGetAllRequestDto
    )
    {
        var limit = postGetAllRequestDto.Limit;

        // Fetch one extra item to determine if there's a next page.
        postGetAllRequestDto.Limit++;
        var posts = await _postRepository.GetAllPosts(postGetAllRequestDto);
        postGetAllRequestDto.Limit--;

        var hasNextPage = posts.Count() > limit;
        var pageItems = posts.Take(limit);

        var response = new PaginatedResponseDto<PostResponseDto>
        {
            Items = await Task.WhenAll(
                pageItems.Select(async p =>
                    p.ToDto(_requestingUser.UserId, await _policyService.CanMutateThisPost(p))
                )
            ),
            NextCursor = hasNextPage ? pageItems.LastOrDefault()?.Id : null,
        };

        return Result<PaginatedResponseDto<PostResponseDto>>.Success(response);
    }

    public async Task<Result<PostResponseDto>> GetPostById(Guid postId)
    {
        var post = await _postRepository.GetFatPostById(postId);

        if (post == null)
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound);
        }

        return Result<PostResponseDto>.Success(
            post.ToDto(_requestingUser.UserId, await _policyService.CanMutateThisPost(post))
        );
    }

    public async Task<Result<PostResponseDto>> CreatePost(PostCreateRequestDto postCreateRequestDto)
    {
        //Check that user can create posts
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanParticipateInForum())
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.Unauthorized);
        }

        var tags = await _tagRepository.GetTagsByIds(postCreateRequestDto.Tags);

        var createdPost = await _postRepository.CreatePost(
            postCreateRequestDto.CreateDtoToEntity(_requestingUser.UserId.Value, tags.ToList())
        );

        return Result<PostResponseDto>.Success(createdPost.ToDto(_requestingUser.UserId, true));
    }

    public async Task<Result<PostResponseDto>> UpdatePost(
        Guid postId,
        PostUpdateRequestDto postUpdateRequestDto
    )
    {
        //Check that post even exists
        var post = await _postRepository.GetFatPostById(postId);
        if (post == null)
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound);
        }

        //Check that user can mutate the post
        var canMutateThisPost = await _policyService.CanMutateThisPost(post);
        if (!canMutateThisPost)
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.PostUnauthorized);
        }

        var tags =
            postUpdateRequestDto.Tags != null
                ? await _tagRepository.GetTagsByIds(postUpdateRequestDto.Tags)
                : null;

        postUpdateRequestDto.UpdateDtoToEntity(post, tags?.ToList());
        await _postRepository.SaveChangesAsync();

        return Result<PostResponseDto>.Success(
            post.ToDto(_requestingUser.UserId, canMutateThisPost)
        );
    }

    public async Task<Result<PostResponseDto>> VotePost(
        Guid postId,
        PostVoteRequestDto postVoteRequestDto
    )
    {
        //Check that post exists
        var post = await _postRepository.GetSlimPostbyId(postId);
        if (post == null)
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound);
        }

        //Check that user can vote
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanParticipateInForum())
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.Unauthorized);
        }

        var existingVote = await _postVoteRepository.GetVoteByPostAndUserId(
            postId,
            _requestingUser.UserId.Value
        );
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
                    VoterId = _requestingUser.UserId.Value,
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

        //Check for permissions to put in Dto
        var canMutatePost = await _policyService.CanMutateThisPost(post);

        return Result<PostResponseDto>.Success(post.ToDto(_requestingUser.UserId, canMutatePost));
    }

    public async Task<Result<PostResponseDto>> DeletePost(Guid postId)
    {
        //Check that post exists
        var post = await _postRepository.GetSlimPostbyId(postId);
        if (post == null)
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.PostNotFound);
        }

        //Check that user can delete the post
        var canMutatePost = await _policyService.CanMutateThisPost(post);
        if (!canMutatePost)
        {
            return Result<PostResponseDto>.Failure(ErrorMessages.PostUnauthorized);
        }

        post.IsDeleted = true;
        post.UpdatedAt = DateTime.UtcNow;
        await _postRepository.SaveChangesAsync();

        return Result<PostResponseDto>.Success(post.ToDto(_requestingUser.UserId, canMutatePost));
    }
}
