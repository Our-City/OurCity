import { Period } from "@/types/enums";

export interface AnalyticsSummary {
  period: Period;
  startDate: Date;
  endDate: Date;
  totalPosts: number;
  totalComments: number;
  totalUpvotes: number;
  totalDownvotes: number;
}

export interface AnalyticsTimeSeries {
  period: Period;
  series: {
    bucketStart: Date;
    bucketEnd: Date;
    postCount: number;
  }[];
}

export interface AnalyticsTags {
  period: Period;
  tagBuckets: {
    tagId: string;
    tagName: string;
    postCount: number;
  }[];
}
