using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Authorization.CanMutateThisComment;

public class CanMutateThisCommentHandler : AuthorizationHandler<CanMutateThisCommentRequirement, Comment>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanMutateThisCommentRequirement requirement,
        Comment comment
    )
    {
        var user = context.User;

        if (user.FindFirst(ClaimTypes.NameIdentifier)?.Value == comment.AuthorId.ToString())
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
