using System.Net;
using System.Net.Http.Json;
using OurCity.Api.Common.Dtos.User;

namespace OurCity.Api.Test.IntegrationTests;

[Trait("Type", "Integration")]
[Trait("Domain", "User")]
public class UserIntegrationTests : IAsyncLifetime, IClassFixture<OurCityWebApplicationFactory>
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
    public async Task CreatingUserWithTooLongUsernameFails()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserCreateRequestDto
        {
            Username = "SubterraneanHomesickAlienSubterraneanHomesickAlien1",
            Password = "Femsi1!fm",
        };

        var response = await client.PostAsJsonAsync($"{_baseUrl}/users", userRequest);

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

        var response = await client.PostAsJsonAsync($"{_baseUrl}/users", userRequest);

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

        var response = await client.PostAsJsonAsync($"{_baseUrl}/users", userRequest);

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

        var response = await client.PostAsJsonAsync($"{_baseUrl}/users", userRequest);

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

        var response = await client.PostAsJsonAsync($"{_baseUrl}/users", userRequest);

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

        var response = await client.PostAsJsonAsync($"{_baseUrl}/users", userRequest);
        var responseMessage = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("ValidUser", responseMessage?.Username);
    }

    [Fact]
    public async Task UpdatingNonexistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserUpdateRequestDto { Username = "ThisShouldNotWork" };

        var response = await client.PutAsJsonAsync($"{_baseUrl}/users/{Guid.Empty}", userRequest);

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

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{_ourCityApi.StubUserId}",
            userRequest
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatingExistentUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var userRequest = new UserUpdateRequestDto { Username = "NewUserNameWhoDis" };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{_ourCityApi.StubUserId}",
            userRequest
        );
        var responseMessage = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("NewUserNameWhoDis", responseMessage?.Username);
    }

    [Fact]
    public async Task DeletingNonExistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.DeleteAsync($"{_baseUrl}/users/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletingExistentUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.DeleteAsync($"{_baseUrl}/users/{_ourCityApi.StubUserId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReportingExistentUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        var reportRequest = new UserReportRequestDto { Reason = "Test Reporting Reason" };

        // log in as the stubbed user and create a new user to report
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{_ourCityApi.StubUserId2}/reports",
            reportRequest
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReportingNonExistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        var reportRequest = new UserReportRequestDto { Reason = "Test Reporting Reason" };

        var nonExistentId = Guid.NewGuid();

        // log in as the stubbed user and create a new user to report
        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{nonExistentId}/reports",
            reportRequest
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReportingWithoutAuthenticationFails()
    {
        using var client = _ourCityApi.CreateClient();

        var reportRequest = new UserReportRequestDto { Reason = "Test Reporting Reason" };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{_ourCityApi.StubUserId2}/reports",
            reportRequest
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
