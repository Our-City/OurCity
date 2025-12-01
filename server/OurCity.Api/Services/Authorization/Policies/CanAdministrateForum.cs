using Microsoft.AspNetCore.Authorization;

namespace OurCity.Api.Services.Authorization.Policies;

public class CanAdministrateForumRequirement : IAuthorizationRequirement { }

public class CanAdministrateForumHandler : AuthorizationHandler<CanAdministrateForumRequirement>
{
    private readonly ICurrentUser _user;

    public CanAdministrateForumHandler(ICurrentUser user)
    {
        _user = user;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanAdministrateForumRequirement requirement
    )
    {
        if (_user.IsAdmin)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
