import type { Period } from "../enums";

export interface AnalyticsRequestDto {
    period: Period;
}

export interface AnalyticsSummaryResponseDto {
    Period : Period;
    Start : string; // ISO date string
    End : string;   // ISO date string
    TotalPosts : number;
    TotalUpvotes : number;
    TotalDownvotes : number;
    TotalComments : number;
}

export interface AnalyticsTimeSeriesBucketDto {
    BucketStart : string; // ISO date string
    BucketEnd : string;   // ISO date string
    PostCount : number;
}

export interface AnalyticsTimeSeriesResponseDto {
    Period : Period;
    Buckets : AnalyticsTimeSeriesBucketDto[];
}

export interface AnalyticsTagBucketDto {
    TagId : string;
    TagName : string;
    PostCount : number;
}

export interface AnalyticsTagsResponseDto {
    Period : Period;
    TagBuckets : AnalyticsTagBucketDto[];
}