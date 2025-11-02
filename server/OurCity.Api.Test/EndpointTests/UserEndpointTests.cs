using System.Net;
using System.Net.Http.Json;
using OurCity.Api.Common.Dtos.User;

namespace OurCity.Api.Test.EndpointTests;

[Trait("Type", "Endpoint")]
[Trait("Domain", "User")]
public class UserEndpointTests : IAsyncLifetime, IClassFixture<OurCityWebApplicationFactory>
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
    public async Task CreatingUserWithTooLongUsernameFails()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "SubterraneanHomesickAlienSubterraneanHomesickAlien1",
            Password = "Femsi1!fm",
        };

        var response = await client.PostAsJsonAsync("/Users", userRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PasswordMustContainUppercase()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "InvalidUser",
            Password = "femsi1!fm",
        };

        var response = await client.PostAsJsonAsync("/Users", userRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PasswordMustContainLowercase()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "InvalidUser",
            Password = "FEMSI1!FM",
        };

        var response = await client.PostAsJsonAsync("/Users", userRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PasswordMustContainDigit()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "InvalidUser",
            Password = "Femsi!fm",
        };

        var response = await client.PostAsJsonAsync("/Users", userRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PasswordMustContainNonAlphanumericChar()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "InvalidUser",
            Password = "Femsi1fm",
        };

        var response = await client.PostAsJsonAsync("/Users", userRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatingUserWithValidPasswordAndUsernameWorks()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "ValidUser",
            Password = "Femsi1!fm",
        };

        var response = await client.PostAsJsonAsync("/Users", userRequest);
        var responseMessage = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("ValidUser", responseMessage?.Username);
    }

    [Fact]
    public async Task UpdatingNonexistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserUpdateRequestDto { Username = "ThisShouldNotWork" };

        var response = await client.PutAsJsonAsync($"/Users/{Guid.Empty}", userRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatingExistentUserWithTooLongNameFails()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserUpdateRequestDto
        {
            Username = "SubterraneanHomesickAlienSubterraneanHomesickAlien1",
        };

        var response = await client.PutAsJsonAsync($"/Users/{_ourCityApi.StubUserId}", userRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatingExistentUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserUpdateRequestDto { Username = "NewUserNameWhoDis" };

        var response = await client.PutAsJsonAsync($"/Users/{_ourCityApi.StubUserId}", userRequest);
        var responseMessage = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("NewUserNameWhoDis", responseMessage?.Username);
    }

    [Fact]
    public async Task DeletingNonExistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.DeleteAsync($"/Users/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletingExistentUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.DeleteAsync($"/Users/{_ourCityApi.StubUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
