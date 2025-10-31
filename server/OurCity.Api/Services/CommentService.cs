/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface ICommentService
{
    Task<Result<IEnumerable<CommentResponseDto>>> GetCommentsByPostId(Guid? userId, Guid postId);
    Task<Result<CommentResponseDto>> CreateComment(
        Guid userId,
        Guid postId,
        CommentRequestDto commentCreateRequestDto
    );
    Task<Result<CommentResponseDto>> UpdateComment(
        Guid userId,
        Guid commentId,
        CommentRequestDto commentUpdateRequestDto
    );
    Task<Result<CommentResponseDto>> VoteComment(
        Guid userId,
        Guid commentId,
        CommentVoteRequestDto commentVoteRequestDto
    );
    Task<Result<CommentResponseDto>> DeleteComment(Guid userId, Guid commentId);
}

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentVoteRepository _commentVoteRepository;

    public CommentService(ICommentRepository commentRepository, ICommentVoteRepository commentVoteRepository)
    {
        _commentRepository = commentRepository;
        _commentVoteRepository = commentVoteRepository;
    }

    public async Task<Result<IEnumerable<CommentResponseDto>>> GetCommentsByPostId(
        Guid? userId,
        Guid postId
    )
    {
        var comments = await _commentRepository.GetCommentsByPostId(postId);

        return Result<IEnumerable<CommentResponseDto>>.Success(comments.ToDtos(userId));
    }

    public async Task<Result<CommentResponseDto>> CreateComment(
        Guid userId,
        Guid postId,
        CommentRequestDto commentRequestDto
    )
    {
        var createdComment = await _commentRepository.CreateComment(
            commentRequestDto.CreateRequestToEntity(userId, postId)
        );

        return Result<CommentResponseDto>.Success(createdComment.ToDto(userId));
    }

    public async Task<Result<CommentResponseDto>> UpdateComment(
        Guid userId,
        Guid commentId,
        CommentRequestDto commentRequestDto
    )
    {
        var comment = await _commentRepository.GetCommentById(commentId);

        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentNotFound);
        }

        if (userId != comment.AuthorId)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentUnauthorized);
        }

        comment.Content = commentRequestDto.Content ?? comment.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.SaveChangesAsync();

        return Result<CommentResponseDto>.Success(comment.ToDto(userId));
    }

    public async Task<Result<CommentResponseDto>> VoteComment(
        Guid userId,
        Guid commentId,
        CommentVoteRequestDto commentVoteRequestDto
    )
    {
        var comment = await _commentRepository.GetCommentById(commentId);

        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentNotFound);
        }

        var existingVote = await _commentVoteRepository.GetVoteByCommentAndUserId(commentId, userId);
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
                    VoterId = userId,
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

        return Result<CommentResponseDto>.Success(comment.ToDto(userId));
    }

    public async Task<Result<CommentResponseDto>> DeleteComment(Guid userId, Guid commentId)
    {
        var comment = await _commentRepository.GetCommentById(commentId);

        if (comment == null)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentNotFound);
        }

        if (userId != comment.AuthorId)
        {
            return Result<CommentResponseDto>.Failure(ErrorMessages.CommentUnauthorized);
        }

        // soft deletion in db  (mark Comment as deleted)
        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.SaveChangesAsync();

        return Result<CommentResponseDto>.Success(comment.ToDto(userId));
    }
}