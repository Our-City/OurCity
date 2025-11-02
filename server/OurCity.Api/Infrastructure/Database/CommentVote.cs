using OurCity.Api.Common.Enum;

namespace OurCity.Api.Infrastructure.Database;

public class CommentVote
{
    public Guid Id { get; set; }

    public required Guid CommentId { get; set; }

    public required Guid VoterId { get; set; }

    public required VoteType VoteType { get; set; }

    // Navigation Properties
    public User? Voter { get; set; }

    public Comment? Comment { get; set; }
}
