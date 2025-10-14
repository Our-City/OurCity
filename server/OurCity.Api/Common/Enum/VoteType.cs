using System.Text.Json.Serialization;

namespace OurCity.Api.Common.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoteType
{
    Upvote,
    Downvote,
}
