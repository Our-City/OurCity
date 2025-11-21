using Microsoft.AspNetCore.Authorization;

namespace OurCity.Api.Services.Authorization.CanParticipateInForum;

public class CanParticipateInForumHandler : AuthorizationHandler<CanParticipateInForumRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanParticipateInForumRequirement requirement
    )
    {
        var isUserAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

        if (isUserAuthenticated)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}