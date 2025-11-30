import type { Period } from "../enums";

export interface AnalyticsRequestDto {
    period: Period;
}

export interface AnalyticsSummaryResponseDto {
    period : Period;
    start : string; // ISO date string
    end : string;   // ISO date string
    totalPosts : number;
    totalUpvotes : number;
    totalDownvotes : number;
    totalComments : number;
}

export interface AnalyticsTimeSeriesBucketDto {
    bucketStart : string; // ISO date string
    bucketEnd : string;   // ISO date string
    postCount : number;
}

export interface AnalyticsTimeSeriesResponseDto {
    period : Period;
    buckets : AnalyticsTimeSeriesBucketDto[];
}

export interface AnalyticsTagBucketDto {
    tagId : string;
    tagName : string;
    postCount : number;
}

export interface AnalyticsTagsResponseDto {
    period : Period;
    tagBuckets : AnalyticsTagBucketDto[];
}