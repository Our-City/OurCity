using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Authorization.Policies;

public class CanMutateThisCommentRequirement : IAuthorizationRequirement { }

public class CanMutateThisCommentHandler
    : AuthorizationHandler<CanMutateThisCommentRequirement, Comment>
{
    private readonly ICurrentUser _user;

    public CanMutateThisCommentHandler(ICurrentUser user)
    {
        _user = user;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanMutateThisCommentRequirement requirement,
        Comment comment
    )
    {
        if (_user.UserId == comment.AuthorId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
