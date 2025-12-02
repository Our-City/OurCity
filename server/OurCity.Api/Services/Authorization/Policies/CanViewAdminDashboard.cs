using Microsoft.AspNetCore.Authorization;

namespace OurCity.Api.Services.Authorization.Policies;

public class CanViewAdminDashboardRequirement : IAuthorizationRequirement { }

public class CanViewAdminDashboardHandler : AuthorizationHandler<CanViewAdminDashboardRequirement>
{
    private readonly ICurrentUser _user;

    public CanViewAdminDashboardHandler(ICurrentUser user)
    {
        _user = user;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanViewAdminDashboardRequirement requirement
    )
    {
        if (_user.IsAdmin)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
