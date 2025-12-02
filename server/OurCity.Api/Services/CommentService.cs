/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Authorization;
using OurCity.Api.Infrastructure.Database.App;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface ICommentService
{
    Task<Result<PaginatedResponseDto<CommentResponseDto>>> GetCommentsForPost(
        Guid postId,
        Guid? cursor,
        int limit
    );
    Task<Result<CommentResponseDto>> CreateComment(
        Guid postId,
        CommentRequestDto commentCreateRequestDto
    );
    Task<Result<CommentResponseDto>> UpdateComment(
        Guid commentId,
        CommentRequestDto commentUpdateRequestDto
    );
    Task<Result<CommentResponseDto>> VoteComment(
        Guid commentId,
        CommentVoteRequestDto commentVoteRequestDto
    );
    Task<Result<CommentResponseDto>> DeleteComment(Guid commentId);
}

public class CommentService : ICommentService
{
    private readonly ICurrentUser _requestingUser;
    private readonly IPolicyService _policyService;
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentVoteRepository _commentVoteRepository;
    private readonly IPostRepository _postRepository;

    public CommentService(
        ICurrentUser requestingUser,
        IPolicyService policyService,
        ICommentRepository commentRepository,
        ICommentVoteRepository commentVoteRepository,
        IPostRepository postRepository
    )
    {
        _requestingUser = requestingUser;
        _policyService = policyService;
        _commentRepository = commentRepository;
        _commentVoteRepository = commentVoteRepository;
        _postRepository = postRepository;
    }

    public async Task<Result<PaginatedResponseDto<CommentResponseDto>>> GetCommentsForPost(
        Guid postId,
        Guid? cursor,
        int limit
    )
    {
        var post = await _postRepository.GetSlimPostbyId(postId);
        if (post is null)
        {
            return Result<PaginatedResponseDto<CommentResponseDto>>.Failure(
                ErrorMessages.PostNotFound
            );
        }

        var comments = await _commentRepository.GetCommentsForPost(postId, cursor, limit + 1);

        var hasNextPage = comments.Count() > limit;
        var pageItems = comments.Take(limit);

        var response = new PaginatedResponseDto<CommentResponseDto>
        {
            Items = await Task.WhenAll(
                pageItems.Select(async c =>
                    c.ToDto(_requestingUser.UserId, await _policyService.CanMutateThisComment(c))
                )
            ),
            NextCursor = hasNextPage ? pageItems.LastOrDefault()?.Id : null,
        };

        return Result<PaginatedResponseDto<CommentResponseDto>>.Success(response);
    }

    public async Task<Result<CommentResponseDto>> CreateComment(
        Guid postId,
        CommentRequestDto commentRequestDto
    )
    {
        //Check that user can create posts/comments
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanParticipateInForum())
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.Unauthorized);
        }

        //Check that post exists
        var post = await _postRepository.GetSlimPostbyId(postId);
        if (post is null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.PostNotFound);
        }

        //Create comment
        var createdComment = await _commentRepository.CreateComment(
            commentRequestDto.CreateRequestToEntity(_requestingUser.UserId.Value, postId)
        );

        return Result<CommentResponseDto>.Success(
            createdComment.ToDto(_requestingUser.UserId, canMutate: true)
        );
    }

    public async Task<Result<CommentResponseDto>> UpdateComment(
        Guid commentId,
        CommentRequestDto commentRequestDto
    )
    {
        //Check that the comment exists
        var comment = await _commentRepository.GetCommentById(commentId);
        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentNotFound);
        }

        //Check that user can modify this comment
        var canMutateComment = await _policyService.CanMutateThisComment(comment);
        if (!canMutateComment)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentUnauthorized);
        }

        comment.Content = commentRequestDto.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.SaveChangesAsync();

        return Result<CommentResponseDto>.Success(
            comment.ToDto(_requestingUser.UserId, canMutateComment)
        );
    }

    public async Task<Result<CommentResponseDto>> VoteComment(
        Guid commentId,
        CommentVoteRequestDto commentVoteRequestDto
    )
    {
        //Check that user can vote
        if (!_requestingUser.UserId.HasValue || !await _policyService.CanParticipateInForum())
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.Unauthorized);
        }

        //Check that the comment even exists
        var comment = await _commentRepository.GetCommentById(commentId);
        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentNotFound);
        }

        var existingVote = await _commentVoteRepository.GetVoteByCommentAndUserId(
            commentId,
            _requestingUser.UserId.Value
        );
        var requestedVoteType = commentVoteRequestDto.VoteType;

        if (existingVote != null && requestedVoteType == VoteType.NoVote)
        {
            await _commentVoteRepository.Remove(existingVote);
        }
        else if (existingVote == null && requestedVoteType != VoteType.NoVote)
        {
            await _commentVoteRepository.Add(
                new CommentVote
                {
                    Id = Guid.NewGuid(),
                    CommentId = commentId,
                    VoterId = _requestingUser.UserId.Value,
                    VoteType = requestedVoteType,
                }
            );
        }
        else if (existingVote != null && requestedVoteType != VoteType.NoVote)
        {
            existingVote.VoteType = requestedVoteType;
        }

        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.SaveChangesAsync();

        //Check for permisisons for Dto response
        var canMutateComment = await _policyService.CanMutateThisComment(comment);

        return Result<CommentResponseDto>.Success(
            comment.ToDto(_requestingUser.UserId, canMutateComment)
        );
    }

    public async Task<Result<CommentResponseDto>> DeleteComment(Guid commentId)
    {
        //Check that the comment even exists
        var comment = await _commentRepository.GetCommentById(commentId);
        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentNotFound);
        }

        //Check that user can delete this comment
        var canMutateComment = await _policyService.CanMutateThisComment(comment);
        if (!canMutateComment)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.Unauthorized);
        }

        // soft deletion in db (mark Comment as deleted)
        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.SaveChangesAsync();

        return Result<CommentResponseDto>.Success(
            comment.ToDto(_requestingUser.UserId, canMutateComment)
        );
    }
}
