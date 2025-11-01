/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Mappings;

public static class CommentMappings
{
    public static IEnumerable<CommentResponseDto> ToDtos(
        this IEnumerable<Comment> comments,
        Guid? userId
    )
    {
        return comments.Select(comment => comment.ToDto(userId));
    }

    public static CommentResponseDto ToDto(this Comment comment, Guid? userId)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            UpvoteCount = comment.Votes.Count(vote => vote.VoteType == VoteType.Upvote),
            DownvoteCount = comment.Votes.Count(vote => vote.VoteType == VoteType.Downvote),
            VoteStatus = userId.HasValue
                ? comment.Votes.FirstOrDefault(vote => vote.VoterId.Equals(userId))?.VoteType
                ?? VoteType.NoVote
                : VoteType.NoVote,
            IsDeleted = comment.IsDeleted,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
        };
    }

    public static Comment CreateRequestToEntity(
        this CommentRequestDto commentRequestDto,
        Guid authorId,
        Guid postId
    )
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            AuthorId = authorId,
            Content = commentRequestDto.Content,
            Votes = new(),
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }
}
