using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Infrastructure.Database.App;

namespace OurCity.Api.Services.Authorization.Policies;

public class CanMutateThisPostRequirement : IAuthorizationRequirement { }

public class CanMutateThisPostHandler : AuthorizationHandler<CanMutateThisPostRequirement, Post>
{
    private readonly ICurrentUser _user;

    public CanMutateThisPostHandler(ICurrentUser user)
    {
        _user = user;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanMutateThisPostRequirement requirement,
        Post post
    )
    {
        if (_user.UserId == post.AuthorId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
