using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Comments;

public class CommentVoteRequestDto
{
    public required Guid UserId { get; set; }

    public required VoteType VoteType { get; set; }
}
