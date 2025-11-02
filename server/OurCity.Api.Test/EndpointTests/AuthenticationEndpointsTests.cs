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

    [Fact]
    public async Task GetMeWithLogin()
    {
        using var client = _factory.CreateClient();

        await client.PostAsync("/Authentication/Login/username", null);

        var response = await client.GetAsync("/Authentication/Me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetMeWithLoginThenLogout()
    {
        using var client = _factory.CreateClient();

        await client.PostAsync("/Authentication/Login/username", null);
        var loginMeResponse = await client.GetAsync("/Authentication/Me");
        Assert.Equal(HttpStatusCode.OK, loginMeResponse.StatusCode);

        await client.PostAsync("/Authentication/Logout", null);
        var logoutMeResponse = await client.GetAsync("/Authentication/Me");
        Assert.Equal(HttpStatusCode.Unauthorized, logoutMeResponse.StatusCode);
    }
}
