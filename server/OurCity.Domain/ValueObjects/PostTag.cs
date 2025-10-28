namespace OurCity.Domain.ValueObjects;

public class PostTag
{
    public Guid Guid { get; private set; }
    public string Name { get; private set; }

    private PostTag() { } //EFCore

    public PostTag(string name)
    {
        ValidateName(name);

        Guid = Guid.NewGuid();
        Name = name;
    }

    public void ValidateName(string name)
    {
        if (name.Length > 20)
            throw new InvalidOperationException("Tag name cannot exceed 20 characters.");
    }
}