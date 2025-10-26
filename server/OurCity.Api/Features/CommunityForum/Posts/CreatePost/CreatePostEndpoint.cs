using System.Security.Claims;
using FluentValidation;

namespace OurCity.Api.Features.CommunityForum.Posts.CreatePost;

/// <credits>
/// General structure inspired by post here: https://www.milanjovanovic.tech/blog/vertical-slice-architecture-structuring-vertical-slices
/// Structure is also used for all other endpoints
/// </credits>
public static class CreatePostEndpoint
{
    public static void MapEndpoint(this IEndpointRouteBuilder app, string route)
    {
        app.MapPost(route, Execute)
            .WithSummary("Create post")
            .WithDescription("Create a new post");
    }

    private static IResult Execute(
        CreatePostRequest request,
        IValidator<CreatePostRequest> requestValidator,
        CreatePostHandler handler,
        ClaimsPrincipal user
    )
    {
        var validationResult = requestValidator.Validate(request);
        if (!validationResult.IsValid)
            return TypedResults.BadRequest(validationResult.Errors);

        var response = handler.HandleCreatePostRequest(request, user);
        return TypedResults.Ok(response);
    }
}
