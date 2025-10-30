using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Post;

public class PostVoteRequestDto
{
    public required VoteType VoteType { get; set; }
}
