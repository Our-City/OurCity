namespace OurCity.Domain.DomainEntities;

public class Post
{
    public Guid Guid { get; private set; } //TODO: why GUID
    public string Content { get; private set; }

    //TODO: post views for engagement metrics

    public Post(string content)
    {
        if (content.Length > 100)
            throw new InvalidOperationException("Content must be less than 100 characters.");

        Guid = Guid.NewGuid();
        Content = content;
    }

    public void ChangeContent(string newContent)
    {
        if (newContent.Length > 100)
            throw new InvalidOperationException("Content must be less than 100 characters.");

        Content = newContent;
    }
}
