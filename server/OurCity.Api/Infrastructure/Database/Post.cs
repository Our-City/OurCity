using OurCity.Api.Common.Enum;

namespace OurCity.Api.Infrastructure.Database;

public class Post
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public User Author { get; set; } = null!;
    public Guid AuthorId { get; set; }

    public string? Location { get; set; }

    public List<Guid> UpvotedUserIds { get; set; } = new();
    public List<Guid> DownvotedUserIds { get; set; } = new();
    //public List<Votes> Votes { get; set; } = new();
    public List<int> CommentIds { get; set; } = new();

    public PostVisibility Visibility { get; set; }

    public List<Tags> Tags { get; set; } = new();

    public List<int> MediaAttachments { get; set; } = new();

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Comment> Comments { get; set; } = new();
    public List<Media> Media { get; set; } = new();

}
