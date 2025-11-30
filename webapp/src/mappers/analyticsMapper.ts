import type { AnalyticsSummary, AnalyticsTimeSeries, AnalyticsTags } from "@/models/analytics";
import type { AnalyticsSummaryResponseDto, AnalyticsTimeSeriesResponseDto, AnalyticsTagsResponseDto } from "@/types/dtos/analytics";

// DTOs -> Models:

// maps an AnalyticsSummaryDto to an AnalyticsSummary model
export function toAnalyticsSummary(dto: AnalyticsSummaryResponseDto): AnalyticsSummary {
    return {
        period: dto.Period,
        startDate: new Date(dto.Start),
        endDate: new Date(dto.End),
        totalPosts: dto.TotalPosts,
        totalUpvotes: dto.TotalUpvotes,
        totalDownvotes: dto.TotalDownvotes,
        totalComments: dto.TotalComments,
    };
}

// maps an AnalyticsTimeSeriesDto to an AnalyticsTimeSeries model
export function toAnalyticsTimeSeries(dto: AnalyticsTimeSeriesResponseDto): AnalyticsTimeSeries {
    return {
        period: dto.Period,
        series: dto.Buckets.map(bucket => ({
            bucketStart: new Date(bucket.BucketStart),
            bucketEnd: new Date(bucket.BucketEnd),
            postCount: bucket.PostCount,
        })),
    };
}

// maps an AnalyticsTagsDto to an AnalyticsTags model
export function toAnalyticsTags(dto: AnalyticsTagsResponseDto): AnalyticsTags {
    console.log("Dto:", dto);
    return {
        period: dto.Period,
        tagBuckets: dto.TagBuckets.map(bucket => ({
            tagId: bucket.TagId,
            tagName: bucket.TagName,
            postCount: bucket.PostCount,
        })),
    };
}