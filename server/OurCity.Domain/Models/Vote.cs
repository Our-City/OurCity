using OurCity.Domain.Enums;

namespace OurCity.Domain.Models;

public class Vote
{
    public Guid Guid { get; private set; }
    public Guid PostId { get; private set; }
    public Guid UserId { get; private set; }
    public VoteType VoteType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Vote() { } // EFCore

    public Vote(Guid postId, Guid userId, VoteType voteType)
    {
        Guid = Guid.NewGuid();
        PostId = postId;
        UserId = userId;
        VoteType = voteType;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeVote(VoteType newVote)
    {
        VoteType = newVote;
        UpdatedAt = DateTime.UtcNow;
    }
}
