using System.Net;

namespace OurCity.Api.Test.EndpointTests;

/// <summary>
/// Test the /Authentication endpoints
/// </summary>
/// <credits>
/// Code modified from ChatGPT response just asking how to call API endpoints for testing
/// </credits>
[Trait("Type", "Endpoint")]
[Trait("Domain", "Authentication")]
public class AuthenticationEndpointsTests : IClassFixture<OurCityWebApplicationFactory>
{
    private readonly OurCityWebApplicationFactory _factory;

    public AuthenticationEndpointsTests(OurCityWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMeWithoutLogin()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/Authentication/Me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
