/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common.Dtos.Media;
using OurCity.Api.Common.Dtos.Post;
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
            Votes = post.UpvotedUserIds.Count - post.DownvotedUserIds.Count,
            Location = post.Location,
            Media = post.Media.Select(media => new MediaResponseDto { Url = media.Url }).ToList(),
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
            Media = postCreateRequestDto
                .Media.Select(mediaResponseDto => new Media { Url = mediaResponseDto.Url })
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
        existingPost.Media =
            postUpdateRequestDto.Media.Count != 0
                ? postUpdateRequestDto
                    .Media.Select(mediaDto => new Media { Url = mediaDto.Url })
                    .ToList()
                : existingPost.Media;
        existingPost.UpdatedAt = DateTime.UtcNow;

        return existingPost;
    }
}
