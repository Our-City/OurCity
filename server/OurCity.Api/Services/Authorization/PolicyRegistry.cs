using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Services.Authorization.CanCreatePosts;
using OurCity.Api.Services.Authorization.CanMutateThisPost;

namespace OurCity.Api.Services.Authorization;

/// <summary>
/// Registers the authorization policies for the OurCity application
/// </summary>
/// <credits>
/// Code inspired from my (Andre) WT3
/// </credits>
public class Policy
{
    public static readonly Policy CanCreatePosts = new("CanCreatePosts");
    public static readonly Policy CanMutateThisPost = new("CanMutateThisPost");

    private string Value { get; }

    private Policy(string value) => Value = value;

    public static implicit operator string(Policy p) => p.Value;
}

public static class PolicyRegistry
{
    public static AuthorizationOptions AddOurCityPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(
            Policy.CanCreatePosts,
            policy => policy.Requirements.Add(new CanCreatePostsRequirement())
        );

        options.AddPolicy(
            Policy.CanMutateThisPost,
            policy => policy.Requirements.Add(new CanMutateThisPostRequirement())
        );

        return options;
    }
}
