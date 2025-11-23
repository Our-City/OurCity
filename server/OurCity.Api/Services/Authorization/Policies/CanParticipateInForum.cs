using Microsoft.AspNetCore.Authorization;

namespace OurCity.Api.Services.Authorization.Policies;

public class CanParticipateInForumRequirement : IAuthorizationRequirement { }

public class CanParticipateInForumHandler : AuthorizationHandler<CanParticipateInForumRequirement>
{
    private readonly ICurrentUser _user;

    public CanParticipateInForumHandler(ICurrentUser user)
    {
        _user = user;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanParticipateInForumRequirement requirement
    )
    {
        if (_user.IsAuthenticated)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
