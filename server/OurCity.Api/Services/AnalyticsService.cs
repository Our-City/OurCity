using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurCity.Api.Common;
using OurCity.Api.Common.Dtos.Analytics;
using OurCity.Api.Common.Enum;
using OurCity.Api.Infrastructure;
using OurCity.Api.Infrastructure.Database;
using OurCity.Api.Services.Mappings;

namespace OurCity.Api.Services;

public interface IAnalyticsService
{
    Task<Result<AnalyticsSummaryResponseDto>> GetActivityMetricsSummary(AnalyticsRequestDto analyticsRequestDto);
    Task<Result<AnalyticsTimeSeriesResponseDto>> GetActivityMetricsTimeSeries(AnalyticsRequestDto analyticsRequestDto);
    Task<Result<AnalyticsTagsResponseDto>> GetActivityMetricsTagBreakdown(AnalyticsRequestDto analyticsRequestDto);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly IPostRepository _postRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IPostVoteRepository _postVoteRepository;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(
        IPostRepository postRepository, 
        ITagRepository tagRepository, 
        IPostVoteRepository postVoteRepository,
        ILogger<AnalyticsService> logger)
    {
        _postRepository = postRepository;
        _tagRepository = tagRepository;
        _postVoteRepository = postVoteRepository;
        _logger = logger;
    }

    public async Task<Result<AnalyticsSummaryResponseDto>> GetActivityMetricsSummary(AnalyticsRequestDto analyticsRequestDto)
    {
        var endDate = DateTime.UtcNow;
        var startDate = analyticsRequestDto.Period switch
        {
            Period.Day => endDate.AddDays(-1),
            Period.Month => endDate.AddMonths(-1),
            Period.Year => endDate.AddYears(-1),
            _ => endDate.AddDays(-1)
        };

        var allPosts = await _postRepository.GetAllPosts(new Common.Dtos.Post.PostGetAllRequestDto
        {
            Limit = int.MaxValue,
            SortBy = "date",
            SortOrder = SortOrder.Desc,
            Cursor = null,
        });

        // Fetch total posts
        var totalPosts = allPosts.Count(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate);

        // Fetch total upvotes and downvotes
        var totalUpvotes = allPosts.Sum(p => p.Votes.Count(v => v.VoteType == VoteType.Upvote && v.VotedAt >= startDate && v.VotedAt <= endDate));
        var totalDownvotes = allPosts.Sum(p => p.Votes.Count(v => v.VoteType == VoteType.Downvote && v.VotedAt >= startDate && v.VotedAt <= endDate));
        // Fetch total comments
        var totalComments = allPosts.Sum(p => p.Comments.Count(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate));

        var summary = new AnalyticsSummaryResponseDto
        {
            Period = analyticsRequestDto.Period,
            Start = startDate,
            End = endDate,
            TotalPosts = totalPosts,
            TotalUpvotes = totalUpvotes,
            TotalDownvotes = totalDownvotes,
            TotalComments = totalComments
        };

        return Result<AnalyticsSummaryResponseDto>.Success(summary);
    }

    public async Task<Result<AnalyticsTimeSeriesResponseDto>> GetActivityMetricsTimeSeries(AnalyticsRequestDto analyticsRequestDto)
    {
        var now = DateTime.UtcNow;
        var endDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);
        var startDate = analyticsRequestDto.Period switch
        {
            Period.Day => endDate.AddDays(-1),
            Period.Month => endDate.AddMonths(-1),
            Period.Year => endDate.AddYears(-1),
            _ => endDate.AddDays(-1)
        };

        var allPosts = await _postRepository.GetAllPosts(new Common.Dtos.Post.PostGetAllRequestDto
        {
            Limit = int.MaxValue,
            SortBy = "date",
            SortOrder = SortOrder.Desc,
            Cursor = null,
        });

        // Filter posts within the date range
        var postsInRange = allPosts.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate).ToList();

        var buckets = new List<AnalyticsTimeSeriesBucketDto>();

        // Generate buckets based on the period
        if (analyticsRequestDto.Period == Period.Day)
        {
            // Hourly buckets for the last 24 hours
            for (int i = 23; i >= 0; i--)
            {
                var bucketStart = endDate.AddHours(-i - 1);
                var bucketEnd = endDate.AddHours(-i);
                var postCount = postsInRange.Count(p => p.CreatedAt >= bucketStart && p.CreatedAt < bucketEnd);

                buckets.Add(new AnalyticsTimeSeriesBucketDto
                {
                    BucketStart = bucketStart,
                    BucketEnd = bucketEnd,
                    PostCount = postCount
                });
            }
        }
        else if (analyticsRequestDto.Period == Period.Month)
        {
            // Daily buckets for the last 30 days
            for (int i = 29; i >= 0; i--)
            {
                var bucketStart = endDate.Date.AddDays(-i - 1);
                var bucketEnd = endDate.Date.AddDays(-i);
                var postCount = postsInRange.Count(p => p.CreatedAt >= bucketStart && p.CreatedAt < bucketEnd);

                buckets.Add(new AnalyticsTimeSeriesBucketDto
                {
                    BucketStart = bucketStart,
                    BucketEnd = bucketEnd,
                    PostCount = postCount
                });
            }
        }
        else if (analyticsRequestDto.Period == Period.Year)
        {
            // Monthly buckets for the last 12 months
            for (int i = 11; i >= 0; i--)
            {
                var bucketStart = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-i - 1);
                var bucketEnd = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(-i);
                var postCount = postsInRange.Count(p => p.CreatedAt >= bucketStart && p.CreatedAt < bucketEnd);

                buckets.Add(new AnalyticsTimeSeriesBucketDto
                {
                    BucketStart = bucketStart,
                    BucketEnd = bucketEnd,
                    PostCount = postCount
                });
            }
        }

        var timeSeries = new AnalyticsTimeSeriesResponseDto
        {
            Period = analyticsRequestDto.Period,
            Buckets = buckets
        };

        return Result<AnalyticsTimeSeriesResponseDto>.Success(timeSeries);
    }

    public async Task<Result<AnalyticsTagsResponseDto>> GetActivityMetricsTagBreakdown(AnalyticsRequestDto analyticsRequestDto)
    {
        var endDate = DateTime.UtcNow;
        var startDate = analyticsRequestDto.Period switch
        {
            Period.Day => endDate.AddDays(-1),
            Period.Month => endDate.AddMonths(-1),
            Period.Year => endDate.AddYears(-1),
            _ => endDate.AddDays(-1)
        };

        _logger.LogInformation("Getting all posts for tag breakdown from {StartDate} to {EndDate}", startDate, endDate);

        var allPosts = await _postRepository.GetAllPosts(new Common.Dtos.Post.PostGetAllRequestDto
        {
            Limit = int.MaxValue,
            SortBy = "date",
            SortOrder = SortOrder.Desc,
            Cursor = null,
        });

        // Filter posts within the date range
        var postsInRange = allPosts.Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate).ToList();

        var tags = postsInRange.SelectMany(p => p.Tags).Distinct().ToList();

        _logger.LogInformation("Filtered posts tags from {StartDate} to {EndDate}: {tags}", startDate, endDate, tags);
        // Group posts by tag and count
        var tagCounts = postsInRange
            .SelectMany(p => p.Tags)
            .GroupBy(t => new { t.Id, t.Name })
            .Select(g => new AnalyticsTagBucketDto
            {
                TagID = g.Key.Id,
                TagName = g.Key.Name,
                PostCount = g.Count()
            })
            .OrderByDescending(t => t.PostCount)
            .ToList();

        _logger.LogInformation("Calculated tag counts for period {Period}: {@TagCounts}", analyticsRequestDto.Period, tagCounts);

        var tagBreakdown = new AnalyticsTagsResponseDto
        {
            Period = analyticsRequestDto.Period,
            TagBuckets = tagCounts
        };

        return Result<AnalyticsTagsResponseDto>.Success(tagBreakdown);
    }
}