using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Authorization;

/// <summary>
/// Checks policies using Microsoft's AuthorizationService
/// </summary>
/// <credits>
/// Code was refined by ChatGPT
/// </credits>
public interface IPolicyService
{
    Task<bool> CanParticipateInForum();
    Task<bool> CanMutateThisPost(Post post);
    Task<bool> CanMutateThisComment(Comment comment);
    Task<bool> CanAdministrateForum();
    Task<bool> CanViewAdminDashboard();
}

public class PolicyService : IPolicyService
{
    private readonly ICurrentUser _user;
    private readonly IAuthorizationService _authorizationService;

    public PolicyService(ICurrentUser user, IAuthorizationService authorizationService)
    {
        _user = user;
        _authorizationService = authorizationService;
    }

    public async Task<bool> CanParticipateInForum()
    {
        var authResult = await _authorizationService.AuthorizeAsync(
            _user.Principal,
            Policy.CanParticipateInForum
        );
        return authResult.Succeeded;
    }

    public async Task<bool> CanMutateThisPost(Post post)
    {
        var authResult = await _authorizationService.AuthorizeAsync(
            _user.Principal,
            post,
            Policy.CanMutateThisPost
        );
        return authResult.Succeeded;
    }

    public async Task<bool> CanMutateThisComment(Comment comment)
    {
        var authResult = await _authorizationService.AuthorizeAsync(
            _user.Principal,
            comment,
            Policy.CanMutateThisComment
        );
        return authResult.Succeeded;
    }

    public async Task<bool> CanAdministrateForum()
    {
        var authResult = await _authorizationService.AuthorizeAsync(
            _user.Principal,
            Policy.CanAdministrateForum
        );
        return authResult.Succeeded;
    }

    public async Task<bool> CanViewAdminDashboard()
    {
        var authResult = await _authorizationService.AuthorizeAsync(
            _user.Principal,
            Policy.CanViewAdminDashboard
        );
        return authResult.Succeeded;
    }
}
