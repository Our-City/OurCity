using OurCity.Api.Common.Enum;

namespace OurCity.Api.Infrastructure.Database;

public class PostVote
{
    public Guid Id { get; set; }

    public required Guid PostId { get; set; }

    public required Guid VoterId { get; set; }

    public required VoteType VoteType { get; set; }

    public required DateTime VotedAt { get; set; }
    
    // Navigation Properties
    public User? Voter { get; set; }

    public Post? Post { get; set; }
}