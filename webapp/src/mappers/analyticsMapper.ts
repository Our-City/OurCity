import type { AnalyticsSummary, AnalyticsTimeSeries, AnalyticsTags } from "@/models/analytics";
import type {
  AnalyticsSummaryResponseDto,
  AnalyticsTimeSeriesResponseDto,
  AnalyticsTagsResponseDto,
} from "@/types/dtos/analytics";

// DTOs -> Models:

// maps an AnalyticsSummaryDto to an AnalyticsSummary model
export function toAnalyticsSummary(dto: AnalyticsSummaryResponseDto): AnalyticsSummary {
  return {
    period: dto.period,
    startDate: new Date(dto.start),
    endDate: new Date(dto.end),
    totalPosts: dto.totalPosts,
    totalUpvotes: dto.totalUpvotes,
    totalDownvotes: dto.totalDownvotes,
    totalComments: dto.totalComments,
  };
}

// maps an AnalyticsTimeSeriesDto to an AnalyticsTimeSeries model
export function toAnalyticsTimeSeries(dto: AnalyticsTimeSeriesResponseDto): AnalyticsTimeSeries {
  return {
    period: dto.period,
    series: dto.buckets.map((bucket) => ({
      bucketStart: new Date(bucket.bucketStart),
      bucketEnd: new Date(bucket.bucketEnd),
      postCount: bucket.postCount,
    })),
  };
}

// maps an AnalyticsTagsDto to an AnalyticsTags model
export function toAnalyticsTags(dto: AnalyticsTagsResponseDto): AnalyticsTags {
  return {
    period: dto.period,
    tagBuckets: dto.tagBuckets.map((bucket) => ({
      tagId: bucket.tagId,
      tagName: bucket.tagName,
      postCount: bucket.postCount,
    })),
  };
}
