using OurCity.Api.Common.Dtos;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Mappings;

public static class PostMappings
{
    public static IEnumerable<PostResponseDto> ToDtos(this IEnumerable<Post> posts)
    {
        return posts.Select(post => post.ToDto());
    }

    public static PostResponseDto ToDto(this Post post)
    {
        return new PostResponseDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            Title = post.Title,
            Description = post.Description,
            UpvotedUserIds = post.UpvotedUserIds,
            DownvotedUserIds = post.DownvotedUserIds,
            Location = post.Location,
            Images = post.Images.Select(image => new ImageDto { Url = image.Url }).ToList(),
            CommentIds = post.Comments?.Select(c => c.Id).ToList() ?? new List<int>(),
        };
    }

    public static Post CreateDtoToEntity(this PostCreateRequestDto postCreateRequestDto)
    {
        return new Post
        {
            Title = postCreateRequestDto.Title,
            Description = postCreateRequestDto.Description,
            Location = postCreateRequestDto.Location,
            AuthorId = postCreateRequestDto.AuthorId,
            Images = postCreateRequestDto
                .Images.Select(imgDto => new Image { Url = imgDto.Url })
                .ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static Post UpdateDtoToEntity(
        this PostUpdateRequestDto postUpdateRequestDto,
        Post existingPost
    )
    {
        existingPost.Title = postUpdateRequestDto.Title ?? existingPost.Title;
        existingPost.Description = postUpdateRequestDto.Description ?? existingPost.Description;
        existingPost.Location = postUpdateRequestDto.Location ?? existingPost.Location;
        existingPost.Images =
            postUpdateRequestDto.Images.Count != 0
                ? postUpdateRequestDto
                    .Images.Select(imgDto => new Image { Url = imgDto.Url })
                    .ToList()
                : existingPost.Images;
        existingPost.UpdatedAt = DateTime.UtcNow;

        return existingPost;
    }
}
