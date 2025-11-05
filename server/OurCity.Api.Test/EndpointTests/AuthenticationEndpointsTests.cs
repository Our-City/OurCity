using System.Net;
using System.Net.Http.Json;
using OurCity.Api.Common.Dtos.User;

namespace OurCity.Api.Test.EndpointTests;

/// <summary>
/// Test the /authentication endpoints
/// </summary>
/// <credits>
/// Code modified from ChatGPT response just asking how to call API endpoints for testing
/// </credits>
[Trait("Type", "Endpoint")]
[Trait("Domain", "Authentication")]
public class AuthenticationEndpointsTests
    : IAsyncLifetime,
        IClassFixture<OurCityWebApplicationFactory>
{
    private OurCityWebApplicationFactory _ourCityApi = null!;
    private readonly string _baseUrl = "/apis/v1";

    public async Task InitializeAsync()
    {
        _ourCityApi = new OurCityWebApplicationFactory();
        await _ourCityApi.StartDbAsync();
    }

    public async Task DisposeAsync()
    {
        await _ourCityApi.StopDbAsync();
    }

    [Fact]
    public async Task GetMeWithoutLogin()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.GetAsync($"{_baseUrl}/authentication/me");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LoggingInToExistentAccountWorks()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/register", userRequest);

        var loginResponse = await client.PostAsJsonAsync(
            $"{_baseUrl}/authentication/login",
            userRequest
        );
        var meResponse = await client.GetAsync($"{_baseUrl}/authentication/me");

        Assert.Equal(HttpStatusCode.NoContent, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
    }

    [Fact]
    public async Task LoggingInToNonexistentAccountWorks()
    {
        using var client = _ourCityApi.CreateClient();

        var nonExistentUserRequest = new UserCreateRequestDto
        {
            Username = "IDontExist",
            Password = "Femsi1!fm",
        };

        var loginResponse = await client.PostAsJsonAsync(
            $"{_baseUrl}/authentication/login",
            nonExistentUserRequest
        );
        var meResponse = await client.GetAsync($"{_baseUrl}/authentication/me");

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, meResponse.StatusCode);
    }

    [Fact]
    public async Task LoggingInThenLoggingOut()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/register", userRequest);

        var loginResponse = await client.PostAsJsonAsync(
            $"{_baseUrl}/authentication/login",
            userRequest
        );
        var meAfterLoginResponse = await client.GetAsync($"{_baseUrl}/authentication/me");

        var logoutResponse = await client.PostAsync($"{_baseUrl}/authentication/logout", null);
        var meAfterLogoutResponse = await client.GetAsync($"{_baseUrl}/authentication/me");

        Assert.Equal(HttpStatusCode.NoContent, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, meAfterLoginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, meAfterLogoutResponse.StatusCode);
    }
}
