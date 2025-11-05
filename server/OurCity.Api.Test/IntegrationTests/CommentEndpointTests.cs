using System.Net;
using System.Net.Http.Json;
using OurCity.Api.Common.Dtos.Comments;
using OurCity.Api.Common.Dtos.Pagination;
using OurCity.Api.Common.Dtos.User;
using OurCity.Api.Common.Enum;
using OurCity.Api.Test.IntegrationTests;

namespace OurCity.Api.Test.EndpointTests;

[Trait("Type", "Endpoint")]
[Trait("Domain", "Comment")]
public class CommentEndpointTests : IAsyncLifetime, IClassFixture<OurCityWebApplicationFactory>
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

    private async Task LoginOnClient(HttpClient client)
    {
        var loginRequest = new UserCreateRequestDto
        {
            Username = _ourCityApi.StubUsername,
            Password = _ourCityApi.StubPassword,
        };

        await client.PostAsJsonAsync($"{_baseUrl}/authentication/login", loginRequest);
    }

    [Fact]
    public async Task GettingCommentsForNonexistentPost()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.GetAsync($"{_baseUrl}/posts/{Guid.Empty}/comments");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GettingCommentsForPostWithNoComments()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.GetAsync($"{_baseUrl}/posts/{_ourCityApi.StubPostId}/comments");
        var responseContent = await response.Content.ReadFromJsonAsync<
            PaginatedResponseDto<CommentResponseDto>
        >();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(0, responseContent?.Items.Count());
    }

    [Fact]
    public async Task CreatingCommentWhenNotLoggedIn()
    {
        using var client = _ourCityApi.CreateClient();

        var createCommentRequest = new CommentRequestDto { Content = "TestContent" };

        var response = await client.PostAsJsonAsync(
            $"{_baseUrl}/posts/{_ourCityApi.StubPostId}/comments",
            createCommentRequest
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatingCommentForNonexistentPost()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var createCommentRequest = new CommentRequestDto { Content = "TestContent" };

        var response = await client.PostAsJsonAsync(
            $"{_baseUrl}/posts/{Guid.Empty}/comments",
            createCommentRequest
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatingCommentWithTooLongContent()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var createCommentRequest = new CommentRequestDto { Content = new string('x', 501) };

        var response = await client.PostAsJsonAsync(
            $"{_baseUrl}/posts/{_ourCityApi.StubPostId}/comments",
            createCommentRequest
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatingCommentForPostAndGettingIt()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var createCommentRequest = new CommentRequestDto { Content = "TestContent" };

        var createCommentResponse = await client.PostAsJsonAsync(
            $"{_baseUrl}/posts/{_ourCityApi.StubPostId}/comments",
            createCommentRequest
        );

        var getCommentResponse = await client.GetAsync(
            $"{_baseUrl}/posts/{_ourCityApi.StubPostId}/comments"
        );
        var getCommentResponseContent = await getCommentResponse.Content.ReadFromJsonAsync<
            PaginatedResponseDto<CommentResponseDto>
        >();

        Assert.Equal(HttpStatusCode.OK, createCommentResponse.StatusCode);
        Assert.Equal(
            createCommentRequest.Content,
            getCommentResponseContent?.Items.First().Content
        );
    }

    [Fact]
    public async Task UpdatingCommentWhenNotLoggedIn()
    {
        using var client = _ourCityApi.CreateClient();

        var updateCommentRequest = new CommentRequestDto { Content = "TestContent" };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{_ourCityApi.StubCommentId}",
            updateCommentRequest
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdatingNonexistentComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var updateCommentRequest = new CommentRequestDto { Content = "TestContent" };

        var updateCommentResponse = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{Guid.Empty}",
            updateCommentRequest
        );

        Assert.Equal(HttpStatusCode.NotFound, updateCommentResponse.StatusCode);
    }

    [Fact]
    public async Task UpdatingCommentWithTooLongContent()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var createCommentRequest = new CommentRequestDto { Content = new string('x', 501) };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{_ourCityApi.StubCommentId}",
            createCommentRequest
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdatingCommentWithValidRequest()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var updateCommentRequest = new CommentRequestDto { Content = new string('x', 500) };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{_ourCityApi.StubCommentId}",
            updateCommentRequest
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.Equal(updateCommentRequest.Content, responseContent?.Content);
    }

    [Fact]
    public async Task VotingForNonexistentComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var voteCommentRequest = new CommentVoteRequestDto { VoteType = VoteType.NoVote };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{Guid.Empty}/votes",
            voteCommentRequest
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpvotingComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var voteCommentRequest = new CommentVoteRequestDto { VoteType = VoteType.Upvote };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{_ourCityApi.StubCommentId}/votes",
            voteCommentRequest
        );
        var responseContent = await response.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.Equal(VoteType.Upvote, responseContent?.VoteStatus);
    }

    [Fact]
    public async Task DownvotingComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var voteCommentRequest = new CommentVoteRequestDto { VoteType = VoteType.Downvote };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{_ourCityApi.StubCommentId}/votes",
            voteCommentRequest
        );
        var responseContent = await response.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.Equal(VoteType.Downvote, responseContent?.VoteStatus);
    }

    [Fact]
    public async Task NoVotingComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var voteCommentRequest = new CommentVoteRequestDto { VoteType = VoteType.NoVote };

        var response = await client.PutAsJsonAsync(
            $"{_baseUrl}/comments/{_ourCityApi.StubCommentId}/votes",
            voteCommentRequest
        );
        var responseContent = await response.Content.ReadFromJsonAsync<CommentResponseDto>();

        Assert.Equal(VoteType.NoVote, responseContent?.VoteStatus);
    }

    [Fact]
    public async Task DeletingNonExistentComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var response = await client.DeleteAsync($"{_baseUrl}/comments/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletingCommentWhenNotLoggedIn()
    {
        using var client = _ourCityApi.CreateClient();

        var response = await client.DeleteAsync($"{_baseUrl}/comments/{Guid.Empty}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletingComment()
    {
        using var client = _ourCityApi.CreateClient();
        await LoginOnClient(client);

        var response = await client.DeleteAsync($"{_baseUrl}/comments/{_ourCityApi.StubCommentId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
