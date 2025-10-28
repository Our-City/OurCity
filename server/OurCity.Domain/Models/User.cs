namespace OurCity.Domain.Models;

public class User
{
    public Guid Guid { get; private set; }
    public string Username { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User() { } //EFCore

    private void ValidateUsername(string username)
    {
        if (username.Length > 50)
            throw new InvalidOperationException("Username must not exceed 50 characters.");
    }
    
    public User(string username)
    {
        ValidateUsername(username);
        
        Guid = Guid.NewGuid();
        Username = username;
        CreatedAt = DateTime.Now;
    }

    public void ChangeUsername(string newUsername)
    {
        ValidateUsername(newUsername);
        
        Username = newUsername;
        UpdatedAt = DateTime.Now;
    }
}
