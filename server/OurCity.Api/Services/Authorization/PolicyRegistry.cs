using Microsoft.AspNetCore.Authorization;
using OurCity.Api.Services.Authorization.Policies;

namespace OurCity.Api.Services.Authorization;

/// <summary>
/// Registers the authorization policies for the OurCity application
/// </summary>
/// <credits>
/// Code inspired from my (Andre) WT3
/// </credits>
public class Policy
{
    public static readonly Policy CanParticipateInForum = new("CanParticipateInForum");
    public static readonly Policy CanMutateThisPost = new("CanMutateThisPost");
    public static readonly Policy CanMutateThisComment = new("CanMutateThisComment");

    private string Value { get; }

    private Policy(string value) => Value = value;

    public static implicit operator string(Policy p) => p.Value;
}

public static class PolicyRegistry
{
    public static AuthorizationOptions AddOurCityPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(
            Policy.CanParticipateInForum,
            policy => policy.Requirements.Add(new CanParticipateInForumRequirement())
        );

        options.AddPolicy(
            Policy.CanMutateThisPost,
            policy => policy.Requirements.Add(new CanMutateThisPostRequirement())
        );

        options.AddPolicy(
            Policy.CanMutateThisComment,
            policy => policy.Requirements.Add(new CanMutateThisCommentRequirement())
        );

        return options;
    }
}
