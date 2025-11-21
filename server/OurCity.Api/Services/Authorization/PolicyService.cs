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
    Task<bool> CheckResourcePolicy(ClaimsPrincipal user, Policy policy, Guid resourceId);
}

public class PolicyService : IPolicyService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public PolicyService(IAuthorizationService authorizationService, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _authorizationService = authorizationService;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task<bool> CheckPolicy(ClaimsPrincipal user, Policy policy)
    {
        var authResult = await _authorizationService.AuthorizeAsync(user, policy);

        var isAllowed = authResult.Succeeded;

        return isAllowed;
    }

    public async Task<bool> CheckResourcePolicy(
        ClaimsPrincipal user,
        Policy policy,
        Guid resourceId
    )
    {
        bool isAllowed = false;

        if (policy == Policy.CanMutateThisPost)
            isAllowed = await CheckCanMutateThisPost(user, resourceId);

        if (policy == Policy.CanMutateThisComment)
            isAllowed = await CheckCanMutateThisComment(user, resourceId);

        return isAllowed;
    }

    private async Task<bool> CheckCanMutateThisPost(ClaimsPrincipal user, Guid postId)
    {
        var post = await _postRepository.GetSlimPostbyId(postId);

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

    private async Task<bool> CheckCanMutateThisComment(ClaimsPrincipal user, Guid commentId)
    {
        var comment = await _commentRepository.GetCommentById(commentId);

        if (comment == null)
            return false;

        var authResult = await _authorizationService.AuthorizeAsync(
            user,
            comment,
            Policy.CanMutateThisComment
        );
        var isAllowed = authResult.Succeeded;

        return isAllowed;
    }
}
