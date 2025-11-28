using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Infrastructure.Database.App;

namespace OurCity.Api.Services.Authorization.CanMutateThisPost;

public class CanMutateThisPostHandler : AuthorizationHandler<CanMutateThisPostRequirement, Post>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanMutateThisPostRequirement requirement,
        Post post
    )
    {
        var user = context.User;

        if (user.FindFirst(ClaimTypes.NameIdentifier)?.Value == post.AuthorId.ToString())
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
