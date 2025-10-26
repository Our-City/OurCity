using System.Security.Claims;

namespace OurCity.Api.Features.CommunityForum.Posts.CreatePost;

public class CreatePostHandler
{
    //can put repos here

    public CreatePostHandler()
    {
        //DI stuff here
    }

    public CreatePostResponse? HandleCreatePostRequest(
        CreatePostRequest request,
        ClaimsPrincipal user
    )
    {
        bool isUserAuthenticated = !user.Identity?.IsAuthenticated ?? false;
        if (!isUserAuthenticated)
            return null;

        return new CreatePostResponse(
            Id: 1,
            AuthorId: 1,
            Title: request.Title,
            Description: request.Description,
            Location: request.Location,
            UpvoteNum: 1,
            DownvoteNum: 1,
            PostVisibility: null,
            PostStatus: "Status",
            PostTag: [],
            CreatedAt: DateTime.Now,
            UpdatedAt: DateTime.Now
        );
    }
}
