namespace OurCity.Api.Common.Dtos.Analytics;

public class AnalyticsTagBucketDto
{
    public Guid TagID { get; set; }
    public string TagName { get; set; }
    public int PostCount { get; set; }
}
