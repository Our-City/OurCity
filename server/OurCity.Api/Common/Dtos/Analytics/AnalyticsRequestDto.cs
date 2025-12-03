using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Analytics;

public class AnalyticsRequestDto
{
    public required Period Period { get; set; } = Period.Day;
}
