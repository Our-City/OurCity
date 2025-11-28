using Microsoft.AspNetCore.Mvc;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Analytics;
using OurCity.Api.Extensions;
using OurCity.Api.Services;

namespace OurCity.Api.Controllers;

[ApiController]
[Route("apis/v1/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly ILogger<AnalyticsController> _logger;
    private readonly IAnalyticsService _analyticsService;
    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    [HttpGet]
    [Route("{summary}")]
    [EndpointSummary("Get activity metrics summary")]
    [EndpointDescription("Gets a summary of activity metrics for analytics purposes")]
    [ProducesResponseType(typeof(AnalyticsSummaryResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivityMetricsSummary([FromBody] AnalyticsRequestDto analyticsRequestDto)
    {
        var res = await _analyticsService.GetActivityMetricsSummary(analyticsRequestDto);

        return Ok(res.Data);
    }

    [HttpGet]
    [Route("{time-series}")]
    [EndpointSummary("Get time series of posts created")]
    [EndpointDescription("Gets a set of time series buckets for posts created over time")]
    [ProducesResponseType(typeof(AnalyticsTimeSeriesResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivityMetricsTimeSeries([FromBody] AnalyticsRequestDto analyticsRequestDto)
    {
        var res = await _analyticsService.GetActivityMetricsTimeSeries(analyticsRequestDto);

        return Ok(res.Data);
    }

    [HttpGet]
    [Route("{tag-breakdown}")]
    [EndpointSummary("Get tag breakdown of posts")]
    [EndpointDescription("Gets a set of post counts broken down by tag")]
    [ProducesResponseType(typeof(AnalyticsTagsResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivityMetricsTagBreakdown([FromBody] AnalyticsRequestDto analyticsRequestDto)
    {
        var res = await _analyticsService.GetActivityMetricsTagBreakdown(analyticsRequestDto);

        return Ok(res.Data);
    }
}