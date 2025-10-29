using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos;

public class PostVoteRequestDto
{
    public required Guid UserId { get; set; }

    public required VoteType VoteType { get; set; }
}
