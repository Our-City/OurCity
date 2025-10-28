namespace OurCity.Domain.Models;

/// <credits>
/// Asked ChatGPT to create domain model for Comment. Then manually added OurCity specific logic
/// </credits>
public class Comment
{
    public Guid Guid { get; private set; }
    public Guid PostId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Comment() { } // EFCore

    public Comment(Guid postId, Guid authorId, string content)
    {
        Guid = Guid.NewGuid();
        PostId = postId;
        AuthorId = authorId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    public void ValidateContent(string content)
    {
        if (content.Length > 500)
            throw new InvalidOperationException("Comment content must not exceed 500 characters.");
    }

    public void UpdateContent(string newContent)
    {
        ValidateContent(newContent);
        
        Content = newContent;
        UpdatedAt = DateTime.UtcNow;
    }
}
