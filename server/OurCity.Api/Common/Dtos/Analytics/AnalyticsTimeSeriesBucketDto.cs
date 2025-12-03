namespace OurCity.Api.Common.Dtos.Analytics;

public class AnalyticsTimeSeriesBucketDto
{
    public DateTime BucketStart { get; set; }
    public DateTime BucketEnd { get; set; }
    public int PostCount { get; set; }
}
