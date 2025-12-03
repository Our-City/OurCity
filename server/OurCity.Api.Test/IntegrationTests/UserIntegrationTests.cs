using System.Net;
using System.Net.Http.Json;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.Post;
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

    [Fact]
    public async Task UnauthenticatedBanningUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var reportRequest = new UserReportRequestDto { Reason = "Test Reporting Reason" };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{_ourCityApi.StubUserId}/reports",
            reportRequest
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UnauthenticatedUnbanningUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/ban",
            null
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task NonAdminBanningUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId2}/ban",
            null
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task NonAdminUnbanningUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId2}/unban",
            null
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminBanningSelfFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.AdminUserId}/ban",
            null
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminUnbanningSelfFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.AdminUserId}/unban",
            null
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminBanningNonExistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync($"{_baseUrl}/admin/users/{Guid.Empty}/ban", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminUnbanningNonExistentUserFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync($"{_baseUrl}/admin/users/{Guid.Empty}/unban", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminBanningUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/ban",
            null
        );
        var responseContent = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseContent?.IsBanned);
    }

    [Fact]
    public async Task AdminBanningAndUnbanningUserSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var banResponse = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/ban",
            null
        );
        var banResponseContent = await banResponse.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, banResponse.StatusCode);
        Assert.True(banResponseContent?.IsBanned);

        var unbanResponse = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/unban",
            null
        );
        var unbanResponseContent = await unbanResponse.Content.ReadFromJsonAsync<UserResponseDto>();

        Assert.Equal(HttpStatusCode.OK, unbanResponse.StatusCode);
        Assert.False(unbanResponseContent?.IsBanned);
    }

    [Fact]
    public async Task GetBannedUsersEmptyWithNoBannedUsers()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.GetAsync($"{_baseUrl}/admin/users/banned");
        var responseContent = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<UserResponseDto>
        >();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(responseContent?.Items ?? []);
    }

    [Fact]
    public async Task GetBannedUsersNotEmptyAfterBanning()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        await client.PutAsync($"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/ban", null);

        var response = await client.GetAsync($"{_baseUrl}/admin/users/banned");
        var responseContent = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<UserResponseDto>
        >();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, responseContent?.Items.Count());
    }

    [Fact]
    public async Task GetHighlyReportedUsersReturnsHighlyReportedUser()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.GetAsync($"{_baseUrl}/admin/users/reported");
        var responseContent = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<UserResponseDto>
        >();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, responseContent?.Items.Count());
    }

    [Fact]
    public async Task PromotingUserAsUnauthenticatedFails()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/promote-to-admin",
            null
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PromotingUserAsNonAdminFails()
    {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var response = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId2}/promote-to-admin",
            null
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PromotingUserAsAdminSucceeds()
    {
        using var client = _ourCityApi.CreateClient();

        //Promote as seeded admin user
        var initialAdminLoginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.AdminUsername,
            Password = _ourCityApi.AdminPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", initialAdminLoginRequest);

        var initialPromoteResponse = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId}/promote-to-admin",
            null
        );

        Assert.Equal(HttpStatusCode.NoContent, initialPromoteResponse.StatusCode);

        //Promote as new admin user
        var newAdminLoginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", newAdminLoginRequest);

        var newPromoteResponse = await client.PutAsync(
            $"{_baseUrl}/admin/users/{_ourCityApi.StubUserId2}/promote-to-admin",
            null
        );

        Assert.Equal(HttpStatusCode.NoContent, newPromoteResponse.StatusCode);
    }

    [Fact]
    public async Task GettingPostOfReportedUserShowReportedStatus() {
        using var client = _ourCityApi.CreateClient();

        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername2,
            Password = _ourCityApi.StubPassword2,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);

        var reportRequest = new UserReportRequestDto { Reason = "Test Reporting Reason" };

        // report subbed user 1 as stubbed user 2
        await client.PutAsJsonAsync(
            $"{_baseUrl}/users/{_ourCityApi.StubUserId}/reports",
            reportRequest
        );

        //have one stubbed post - owner = stubbed user 1
        var response = await client.GetAsync($"{_baseUrl}/posts/");
        var responseContent = await response.Content.ReadFromJsonAsync<PaginatedResponseDto<PostResponseDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseContent?.Items.First().IsReported);
    }
}
