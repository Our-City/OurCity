/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Mappings;

public static class CommentMappings
{
    public static IEnumerable<CommentResponseDto> ToDtos(this IEnumerable<Comment> comments)
    {
        return comments.Select(comment => comment.ToDto());
    }

    public static CommentResponseDto ToDto(this Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            Votes = comment.UpvotedUserIds.Count - comment.DownvotedUserIds.Count,
            IsDeleted = comment.IsDeleted,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
        };
    }

    public static Comment ToEntity(this CommentCreateRequestDto commentCreateRequestDto, int postId)
    {
        return new Comment
        {
            PostId = postId,
            AuthorId = commentCreateRequestDto.AuthorId,
            Content = commentCreateRequestDto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static Comment ToEntity(
        this CommentUpdateRequestDto commentUpdateRequestDto,
        Comment existingComment
    )
    {
        existingComment.Content = commentUpdateRequestDto.Content ?? existingComment.Content;
        existingComment.UpdatedAt = DateTime.UtcNow;
        return existingComment;
    }
}
