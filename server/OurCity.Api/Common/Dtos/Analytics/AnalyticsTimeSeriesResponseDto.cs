using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Analytics;

public class AnalyticsTimeSeriesResponseDto
{
    public Period Period { get; set; }
    public required List<AnalyticsTimeSeriesBucketDto> Buckets { get; set; }
}
