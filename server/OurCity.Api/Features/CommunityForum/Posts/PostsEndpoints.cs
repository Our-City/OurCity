using OurCity.Api.Features.CommunityForum.Posts.CreatePost;

namespace OurCity.Api.Features.CommunityForum.Posts;

public static class PostsEndpoints
{
    public static void MapPostsEndpoints(this WebApplication app)
    {
        var postRoutes = app.MapGroup("/posts");
    }
}
