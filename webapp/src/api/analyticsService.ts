import api from "./axios";
import type { AnalyticsSummaryResponseDto, AnalyticsTimeSeriesResponseDto, AnalyticsTagsResponseDto, AnalyticsRequestDto } from "@/types/dtos/analytics";
import type { AnalyticsSummary, AnalyticsTimeSeries, AnalyticsTags } from "@/models/analytics";
import { toAnalyticsSummary, toAnalyticsTimeSeries, toAnalyticsTags } from "@/mappers/analyticsMapper";
import type { Period } from "@/types/enums";

export async function getAnalyticsSummary(period: Period): Promise<AnalyticsSummary> {
    const dto: AnalyticsRequestDto = { period };
    const response = await api.get<AnalyticsSummaryResponseDto>( `/analytics/summary`, { params: dto });
    return toAnalyticsSummary(response.data);
}

export async function getAnalyticsTimeSeries(period: Period): Promise<AnalyticsTimeSeries> {
    const dto: AnalyticsRequestDto = { period };
    const response = await api.get<AnalyticsTimeSeriesResponseDto>( `/analytics/time-series`, { params: dto });
    return toAnalyticsTimeSeries(response.data);
}

export async function getAnalyticsTags(period: Period): Promise<AnalyticsTags> {
    const dto: AnalyticsRequestDto = { period };
    const response = await api.get<AnalyticsTagsResponseDto>( `/analytics/tag-breakdown`, { params: dto });
    console.log("Response Data:", response.data);
    return toAnalyticsTags(response.data);
}