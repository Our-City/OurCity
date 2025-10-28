using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using OurCity.Api.Extensions;
using OurCity.Domain.Enums;
using OurCity.Domain.Models;
using OurCity.Domain.ValueObjects;

namespace OurCity.Api.Features.CommunityForum.Posts;

public static class CreatePost
{
    #region HTTP
    public static void MapEndpoint(IEndpointRouteBuilder app, string route)
    {
        app.MapPost(route, ExecuteHttpRequest)
            .WithSummary("Create post")
            .WithDescription("Create a new post");
    }

    public static IResult ExecuteHttpRequest(
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
    #endregion

    #region Business
    public static CreatePostResponse? HandleCreatePostRequest(
        CreatePostRequest request,
        ClaimsPrincipal user
    )
    {
        //TODO: have to put actual user id here
        Post post = new Post(Guid.NewGuid(), request.Title, request.Description, PostVisibility.Published);

        return new CreatePostResponse
        (
            Id: post.Guid,
            AuthorId: post.AuthorId,
            Title: post.Title,
            Description: post.Description,
            Location: post.Location,
            UpvoteNum: post.Votes.Count(vote => vote.VoteType == VoteType.UpVote),
            DownvoteNum: post.Votes.Count(vote => vote.VoteType == VoteType.DownVote),
            ReportCount: post.ReportedUsers.Count(),
            CommentCount: post.Comments.Count(),
            PostVisibility: post.PostVisibility,
            PostTags: post.Tags,
            CreatedAt: post.CreatedAt,
            UpdatedAt: post.UpdatedAt
        );
    }
    #endregion

    #region Contract
    public record CreatePostRequest(
        string Title,
        string Description,
        string? Location,
        IEnumerable<PostTag>? Tags
    );

    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(req => req.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(req => req.Title).MaximumLength(50).WithMessage("Title must not exceed 50 characters.");

            RuleFor(req => req.Description).NotEmpty().WithMessage("Description is required");
            RuleFor(req => req.Description).MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }

    public record CreatePostResponse(
        Guid Id,
        Guid AuthorId,
        string Title,
        string Description,
        string? Location,
        int UpvoteNum,
        int DownvoteNum, 
        int ReportCount,
        int CommentCount,
        PostVisibility PostVisibility,
        IEnumerable<PostTag> PostTags,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
    #endregion
}
