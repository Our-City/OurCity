using System.Security.Claims;

namespace OurCity.Api.Services.Authorization;

public interface IRequestingUser
{
    Guid? UserId { get; }
    Task<bool> HasPolicy(Policy policy);
    Task<bool> HasPolicy(Policy policy, Guid resourceId);
}

public class RequestingUser : IRequestingUser
{
    private readonly ClaimsPrincipal _user;
    private readonly IPolicyService _policyService;

    public RequestingUser(IHttpContextAccessor accessor, IPolicyService policyService)
    {
        _user = accessor.HttpContext?.User 
                     ?? new ClaimsPrincipal(new ClaimsIdentity());
        _policyService = policyService;
    }

    public Guid? UserId =>
        Guid.TryParse(_user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : null;
    
    public async Task<bool> HasPolicy(Policy policy) =>
        await _policyService.CheckPolicy(_user, policy);

    public async Task<bool> HasPolicy(Policy policy, Guid resourceId) =>
        await _policyService.CheckResourcePolicy(_user, policy, resourceId);
}