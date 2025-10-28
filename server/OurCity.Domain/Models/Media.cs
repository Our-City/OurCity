namespace OurCity.Domain.Models;

/// <credits>
/// Asked ChatGPT to create domain model for images -> then repurposed to general Media attachments. Then manually added OurCity specific logic
/// </credits>
public class Media
{
    public Guid Guid { get; private set; }
    public Guid PostId { get; private set; }
    public string Url { get; private set; } = null!;

    private Media() { } // EFCore

    public Media(Guid postId, string url)
    {
        Guid = Guid.NewGuid();
        PostId = postId;
        Url = url;
    }
}
