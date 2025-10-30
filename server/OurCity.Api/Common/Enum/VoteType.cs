/// Following url guides how to convert enum value from integer to specified string in a JSON object
/// https://stackoverflow.com/questions/2441290/javascriptserializer-json-serialization-of-enum-as-string
using System.Text.Json.Serialization;

namespace OurCity.Api.Common.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoteType
{
    Upvote = 1,
    NoVote = 0,
    Downvote = -1,
}
