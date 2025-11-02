/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentResponseDto>> GetCommentsByPostId(int postId);
    Task<Result<CommentResponseDto>> GetCommentById(int postId, int commentId);
    Task<Result<CommentUpvoteResponseDto>> GetUserUpvoteStatus(
        int postId,
        int commentId,
        int userId
    );
    Task<Result<CommentDownvoteResponseDto>> GetUserDownvoteStatus(
        int postId,
        int commentId,
        int userId
    );
    Task<Result<CommentResponseDto>> CreateComment(
        int postId,
        CommentCreateRequestDto commentCreateRequestDto
    );
    Task<Result<CommentResponseDto>> UpdateComment(
        int postId,
        int commentId,
        CommentUpdateRequestDto commentUpdateRequestDto
    );
    Task<Result<CommentResponseDto>> VoteComment(
        int postId,
        int commentId,
        int userId,
        VoteType voteType
    );
    Task<Result<CommentResponseDto>> DeleteComment(int postId, int commentId);
}

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsByPostId(int postId)
    {
        return (await _commentRepository.GetCommentsByPostId(postId)).ToDtos();
    }

    public async Task<Result<CommentResponseDto>> GetCommentById(int postId, int commentId)
    {
        var comment = await _commentRepository.GetCommentById(postId, commentId);

        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure("Comment does not exist");
        }

        return Result<CommentResponseDto>.Success(comment.ToDto());
    }

    public async Task<Result<CommentUpvoteResponseDto>> GetUserUpvoteStatus(
        int postId,
        int commentId,
        int userId
    )
    {
        var comment = await _commentRepository.GetCommentById(postId, commentId);

        if (comment == null)
        {
            return Result<CommentUpvoteResponseDto>.Failure("Comment does not exist");
        }

        var isUpvoted = comment.UpvotedUserIds.Contains(userId);
        return Result<CommentUpvoteResponseDto>.Success(
            new CommentUpvoteResponseDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                Upvoted = isUpvoted,
            }
        );
    }

    public async Task<Result<CommentDownvoteResponseDto>> GetUserDownvoteStatus(
        int postId,
        int commentId,
        int userId
    )
    {
        var comment = await _commentRepository.GetCommentById(postId, commentId);

        if (comment == null)
        {
            return Result<CommentDownvoteResponseDto>.Failure("Comment does not exist");
        }

        var isDownvoted = comment.DownvotedUserIds.Contains(userId);
        return Result<CommentDownvoteResponseDto>.Success(
            new CommentDownvoteResponseDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                Downvoted = isDownvoted,
            }
        );
    }

    public async Task<Result<CommentResponseDto>> CreateComment(
        int postId,
        CommentCreateRequestDto commentCreateRequestDto
    )
    {
        var createdComment = await _commentRepository.CreateComment(
            commentCreateRequestDto.ToEntity(postId)
        );
        return Result<CommentResponseDto>.Success(createdComment.ToDto());
    }

    public async Task<Result<CommentResponseDto>> UpdateComment(
        int postId,
        int commentId,
        CommentUpdateRequestDto commentUpdateRequestDto
    )
    {
        var comment = await _commentRepository.GetCommentById(postId, commentId);

        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure("Comment does not exist");
        }

        var updatedComment = await _commentRepository.UpdateComment(
            commentUpdateRequestDto.ToEntity(comment)
        );

        return Result<CommentResponseDto>.Success(updatedComment.ToDto());
    }

    public async Task<Result<CommentResponseDto>> VoteComment(
        int postId,
        int commentId,
        int userId,
        VoteType voteType
    )
    {
        var comment = await _commentRepository.GetCommentById(postId, commentId);

        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure("Comment does not exist");
        }

        var targetList =
            (voteType == VoteType.Upvote) ? comment.UpvotedUserIds : comment.DownvotedUserIds;
        var oppositeList =
            (voteType == VoteType.Upvote) ? comment.DownvotedUserIds : comment.UpvotedUserIds;

        if (!targetList.Remove(userId))
        {
            targetList.Add(userId);
            oppositeList.Remove(userId);
        }

        comment.UpdatedAt = DateTime.UtcNow;

        var updatedComment = await _commentRepository.UpdateComment(comment);
        return Result<CommentResponseDto>.Success(updatedComment.ToDto());
    }

    public async Task<Result<CommentResponseDto>> DeleteComment(int postId, int commentId)
    {
        var existingComment = await _commentRepository.GetCommentById(postId, commentId);
        if (existingComment == null)
        {
            return Result<CommentResponseDto>.Failure("Comment not found.");
        }

        await _commentRepository.DeleteComment(existingComment);
        return Result<CommentResponseDto>.Success(existingComment.ToDto());
    }
}
