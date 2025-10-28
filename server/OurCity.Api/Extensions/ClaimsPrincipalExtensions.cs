using System.Security.Claims;

namespace OurCity.Api.Extensions;

/// <credits>
/// Asked ChatGPT for extension methods around a GUID user id
/// </credits>
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }
    
    public static bool HasValidUserId(this ClaimsPrincipal user)
        => user.GetUserId() != Guid.Empty;
}