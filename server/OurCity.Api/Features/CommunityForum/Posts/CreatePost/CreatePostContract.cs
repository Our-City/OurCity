using FluentValidation;

namespace OurCity.Api.Features.CommunityForum.Posts.CreatePost;

/// <summary>
/// Request + Request validator
/// </summary>
public record CreatePostRequest(string Title, string Description, string? Location, bool IsDraft);

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(req => req.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(req => req.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(req => req.IsDraft).NotEmpty().WithMessage("IsDraft is required");
    }
}

/// <summary>
/// Response
/// </summary>
public record CreatePostResponse(
    int Id,
    int AuthorId,
    string Title,
    string Description,
    string? Location,
    int UpvoteNum,
    int DownvoteNum,
    string? PostVisibility,
    string PostStatus,
    string[] PostTag,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
