using Microsoft.AspNetCore.Authorization;

namespace OurCity.Api.Services.Authorization.Policies;

public class CanAdministrateForumRequirement : IAuthorizationRequirement { }

public class CanAdministrateForumHandler : AuthorizationHandler<CanAdministrateForumRequirement>
{
    private readonly ICurrentUser _user;
    private readonly ILogger<CanAdministrateForumHandler> _logger;

    public CanAdministrateForumHandler(ICurrentUser user, ILogger<CanAdministrateForumHandler> logger)
    {
        _user = user;
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanAdministrateForumRequirement requirement
    )
    {
        _logger.LogInformation("Checking if user can administrate forum. IsAdmin: {IsAdmin}", _user);
        if (_user.IsAdmin)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
