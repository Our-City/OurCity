namespace OurCity.Api.Features.CommunityForum.Posts;

public static class PostsEndpoints
{
    public static void MapPostsEndpoints(this WebApplication app)
    {
        CreatePost.MapEndpoint(app, "/posts");
    }
}
