/// Generative AI - CoPilot was used to assist in the creation of this file.
/// CoPilot assisted by generating boilerplate code for standard mapping functions
/// based on common patterns in C# for mapping between entities and DTOs
using OurCity.Api.Common.Dtos.Post;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure.Database;

namespace OurCity.Api.Services.Mappings;

public static class PostMappings
{
    public static PostResponseDto ToDto(this Post post, Guid? userId, bool canMutate)
    {
        return new PostResponseDto
        {
            Id = post.Id,
            AuthorId = post.AuthorId,
            Title = post.Title,
            Description = post.Description,
            Location = post.Location,
            Latitude = post.Latitude,
            Longitude = post.Longitude,
            AuthorName = post.Author?.UserName,
            UpvoteCount = post.Votes.Count(vote => vote.VoteType == VoteType.Upvote),
            DownvoteCount = post.Votes.Count(vote => vote.VoteType == VoteType.Downvote),
            CommentCount = post.Comments?.Count ?? 0,
            Visibility = post.Visisbility,
            Tags = post.Tags.ToDtos().ToList(),
            VoteStatus = userId.HasValue
                ? post.Votes.FirstOrDefault(vote => vote.VoterId.Equals(userId))?.VoteType
                ?? VoteType.NoVote
                : VoteType.NoVote,
            IsBookmarked = userId.HasValue
                ? post.Bookmarks.Any(b => b.UserId.Equals(userId))
                : false,
            IsReported = post.Author?.ReceivedReports.Any(r => r.ReporterId == userId) ?? false,
            CanMutate = canMutate,
            IsDeleted = post.IsDeleted,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
        };
    }

    public static Post CreateDtoToEntity(
        this PostCreateRequestDto postCreateRequestDto,
        Guid userId,
        List<Tag> tags
    )
    {
        return new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = userId,
            Title = postCreateRequestDto.Title,
            Description = postCreateRequestDto.Description,
            Location = postCreateRequestDto.Location,
            Latitude = postCreateRequestDto.Latitude,
            Longitude = postCreateRequestDto.Longitude,
            Tags = tags,
            Visisbility = PostVisibility.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public static Post UpdateDtoToEntity(
        this PostUpdateRequestDto postUpdateRequestDto,
        Post existingPost,
        List<Tag>? tags
    )
    {
        existingPost.Title = postUpdateRequestDto.Title ?? existingPost.Title;
        existingPost.Description = postUpdateRequestDto.Description ?? existingPost.Description;
        existingPost.Location = postUpdateRequestDto.Location ?? existingPost.Location;
        existingPost.Tags = tags ?? existingPost.Tags;
        existingPost.UpdatedAt = DateTime.UtcNow;

        return existingPost;
    }
}
