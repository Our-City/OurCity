using System.Net;
using System.Net.Http.Json;
using OurCity.Api.Common.Dtos.User;

namespace OurCity.Api.Test.EndpointTests;

[Trait("Type", "Endpoint")]
[Trait("Domain", "Me")]
public class MeEndpointTests : IAsyncLifetime, IClassFixture<OurCityWebApplicationFactory>
{
    private OurCityWebApplicationFactory _ourCityApi = null!;

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

        var response = await client.GetAsync("/me");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMeWithLogin()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync("/Authentication/Login", loginRequest);

        var response = await client.GetAsync("/me");
        var responseContent = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(_ourCityApi.StubUsername, responseContent?.Username);
    }

    [Fact]
    public async Task UpdatingNameWithInvalidUsername()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        var updateRequestWithOneOverCharacterLimit = new UserUpdateRequestDto
        {
            Username = "SubterraneanHomesickAlienSubterraneanHomesickAlien1",
        };

        await client.PostAsJsonAsync("/Authentication/Login", loginRequest);

        var updateResponse = await client.PutAsJsonAsync(
            "/me",
            updateRequestWithOneOverCharacterLimit
        );

        var meResponse = await client.GetAsync("/me");
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        Assert.NotEqual(
            meResponseContent?.Username,
            updateRequestWithOneOverCharacterLimit.Username
        );
    }

    [Fact]
    public async Task UpdatingNameWithValidUsername()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        var updateRequest = new UserUpdateRequestDto { Username = "NewUserName" };

        await client.PostAsJsonAsync("/Authentication/Login", loginRequest);
        await client.PutAsJsonAsync("/me", updateRequest);

        var response = await client.GetAsync("/me");
        var responseContent = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("NewUserName", responseContent?.Username);
    }

    [Fact]
    public async Task DeletingMe()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync("/Authentication/Login", loginRequest);

        var deleteResponse = await client.DeleteAsync("/me");
        var meResponse = await client.GetAsync("/me");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, meResponse.StatusCode);
    }
}
