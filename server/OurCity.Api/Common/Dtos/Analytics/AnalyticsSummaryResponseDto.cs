using System.ComponentModel.DataAnnotations;
using OurCity.Api.Common.Enum;

namespace OurCity.Api.Common.Dtos.Analytics;

public class AnalyticsSummaryResponseDto
{
    public Period Period { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int TotalPosts { get; set; }
    public int TotalUpvotes { get; set; }
    public int TotalDownvotes { get; set; }
    public int TotalComments { get; set; }
}
