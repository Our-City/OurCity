using OurCity.Api.Common.Dtos.Post;

namespace OurCity.Api.Common.Dtos.User;

public class UserResponseDto
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public required List<Guid> PostIds { get; set; } = new();

    public required List<Guid> CommentIds { get; set; } = new();

    public required bool IsAdmin { get; set; }

    public required bool IsBanned { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }
}
