using OurCity.Domain.Enums;
using OurCity.Domain.ValueObjects;

namespace OurCity.Domain.Models;

/// <credits>
/// Asked ChatGPT to create domain model for Post. Then manually added OurCity specific logic
/// </credits>
public class Post
{
    //Properties
    public Guid Guid { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string? Location { get; private set; }
    public PostVisibility PostVisibility { get; private set; }
    private readonly List<PostTag> _tags = new();
    public IReadOnlyCollection<PostTag> Tags => _tags.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    //Model relationships
    private readonly List<User> _reportedUsers = new();
    public IReadOnlyCollection<User> ReportedUsers => _reportedUsers.AsReadOnly();

    private readonly List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<Media> _media = new();
    public IReadOnlyCollection<Media> Media => _media.AsReadOnly();
    private readonly List<Vote> _votes = new();
    public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();

    private Post() { } // EFCore

    public Post(Guid authorId, string title, string description, PostVisibility postVisibility)
    {
        ValidateTitle(title);   
        ValidateDescription(description);

        Guid = Guid.NewGuid();
        AuthorId = authorId;
        Title = title;
        Description = description;
        PostVisibility = postVisibility;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void ValidateTitle(string title)
    {
        if (title.Length > 50)
            throw new InvalidOperationException("Post title must not exceed 50 characters.");
    }
    
    public void ValidateDescription(string description)
    {
        if (description.Length > 500)
            throw new InvalidOperationException("Post content must not exceed 500 characters.");
    }

    public void UpdateTitle(string newTitle)
    {
        ValidateTitle(newTitle);

        Title = newTitle;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string newDescription)
    {
        ValidateDescription(newDescription);

        Description = newDescription;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddReportedUsers(Guid userId, User user)
    {
        _reportedUsers.Add(user);
    }
    
    public void AddComment(Guid userId, string commentContent)
    {
        var comment = new Comment(Guid, userId, commentContent);
        _comments.Add(comment);
    }

    public void AddMedia(string mediaUrl)
    {
        var media = new Media(Guid, mediaUrl);
        _media.Add(media);
    }
    
    public void AddVote(Guid userId, VoteType voteType)
    {
        var vote = new Vote(Guid, userId, voteType);
        _votes.Add(vote);
    }
    
    public void AddTag(string tagName)
    {
        var tag = new PostTag(tagName);
        _tags.Add(tag);
    }
}
