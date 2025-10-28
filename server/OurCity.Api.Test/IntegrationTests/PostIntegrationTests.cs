using System.Net;

namespace OurCity.Api.Test.IntegrationTests;

/// <summary>
/// Test the /Post endpoints
/// </summary>
/// <credits>
/// Code modified from ChatGPT response just asking how to call API endpoints for testing
/// </credits>
[Trait("Type", "Integration")]
[Trait("Domain", "Post")]
public class PostIntegrationTests : IClassFixture<OurCityWebApplicationFactory>
{
    private readonly OurCityWebApplicationFactory _factory;

    public PostIntegrationTests(OurCityWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Test()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}