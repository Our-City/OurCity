import api from "./axios";
import type { AnalyticsSummaryResponseDto, AnalyticsTimeSeriesBucketDto, AnalyticsTimeSeriesResponseDto, AnalyticsTagBucketDto, AnalyticsTagsResponseDto } from "@/types/dtos/analytics";
import type { AnalyticsSummary, AnalyticsTimeSeries, AnalyticsTags } from "@/models/analytics";
import { toAnalyticsSummary, toAnalyticsTimeSeries, toAnalyticsTags } from "@/mappers/analyticsMapper";
import type { Period } from "@/types/enums";

export async function getAnalyticsSummary(period: Period): Promise<AnalyticsSummary> {
    const response = await api.get<AnalyticsSummaryResponseDto>(`/analytics/summary?period=${period}`);
    return toAnalyticsSummary(response.data);
}