using System.Security.Claims;
using FluentValidation;

namespace OurCity.Api.Features.CommunityForum.Posts;

public static class CreatePost
{
    /// <summary>
    /// HTTP request handling
    /// </summary>
    /// <param name="app"></param>
    /// <param name="route"></param>
    public static void MapEndpoint(IEndpointRouteBuilder app, string route)
    {
        app.MapPost(route, Execute)
            .WithSummary("Create post")
            .WithDescription("Create a new post");
    }

    public static IResult Execute(
        CreatePostRequest request,
        IValidator<CreatePostRequest> requestValidator,
        ClaimsPrincipal user
    )
    {
        var validationResult = requestValidator.Validate(request);
        if (!validationResult.IsValid)
            return TypedResults.BadRequest(validationResult.Errors);

        var response = HandleCreatePostRequest(request, user);
        return TypedResults.Ok(response);
    }

    /// <summary>
    /// Use case handler
    /// </summary>
    public static CreatePostResponse? HandleCreatePostRequest(
        CreatePostRequest request,
        ClaimsPrincipal user
    )
    {
        bool isUserAuthenticated = !user.Identity?.IsAuthenticated ?? false;
        if (!isUserAuthenticated)
            return null;

        return new CreatePostResponse(
            Id: 1,
            AuthorId: 1,
            Title: request.Title,
            Description: request.Description,
            Location: request.Location,
            UpvoteNum: 1,
            DownvoteNum: 1,
            PostVisibility: null,
            PostStatus: "Status",
            PostTag: [],
            CreatedAt: DateTime.Now,
            UpdatedAt: DateTime.Now
        );
    }
    
    /// <summary>
    /// Request/Response Contract
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
}