using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Analytics;
    
public class AnalyticsTagsResponseDto
{
    public Period Period { get; set; }
    public required List<AnalyticsTagBucketDto> Tags { get; set; }
}
