using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Infrastructure;

namespace OurCity.Api.Services.Authorization;

/// <summary>
/// Checks policies using Microsoft's AuthorizationService
/// </summary>
/// <credits>
/// Code was refined by ChatGPT
/// </credits>
public interface IPolicyService
{
    Task<bool> CheckPolicy(ClaimsPrincipal user, Policy policy);
    Task<bool> CheckResourcePolicy(ClaimsPrincipal user, Policy policy, object resource);
}

public class PolicyService : IPolicyService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IPostRepository _postRepository;

    public PolicyService(IAuthorizationService authorizationService, IPostRepository postRepository)
    {
        _authorizationService = authorizationService;
        _postRepository = postRepository;
    }

    public async Task<bool> CheckPolicy(ClaimsPrincipal user, Policy policy)
    {
        var authResult = await _authorizationService.AuthorizeAsync(user, Policy.CanCreatePosts);

        var isAllowed = authResult.Succeeded;

        return isAllowed;
    }

    public async Task<bool> CheckResourcePolicy(
        ClaimsPrincipal user,
        Policy policy,
        object resource
    )
    {
        bool isAllowed = false;

        if (policy == Policy.CanMutateThisPost)
            isAllowed = await CheckCanMutateThisPost(user, policy, (int)resource);

        return isAllowed;
    }

    public async Task<bool> CheckCanMutateThisPost(ClaimsPrincipal user, Policy policy, int postId)
    {
        var post = await _postRepository.GetPostById(postId);

        if (post == null)
            return false;

        var authResult = await _authorizationService.AuthorizeAsync(
            user,
            post,
            Policy.CanMutateThisPost
        );
        var isAllowed = authResult.Succeeded;

        return isAllowed;
    }
}
