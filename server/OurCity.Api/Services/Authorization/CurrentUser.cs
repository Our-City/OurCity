using System.Security.Claims;
using OurCity.Api.Common;
using OurCity.Api.Extensions;

namespace OurCity.Api.Services.Authorization;

public interface ICurrentUser
{
    ClaimsPrincipal Principal { get; }
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly ClaimsPrincipal _user;

    public CurrentUser(IHttpContextAccessor accessor)
    {
        _user = accessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
    }

    public ClaimsPrincipal Principal => _user;

    public Guid? UserId => _user.GetUserId();

    public bool IsAuthenticated => _user.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => _user.IsInRole(UserRoles.Admin);
}
