using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Comments;

public class CommentVoteRequestDto
{
    public required VoteType VoteType { get; set; }
}
