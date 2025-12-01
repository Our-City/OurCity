/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import PostsOverTime from "@/components/admin/Charts/PostsOverTime.vue";
import { Period } from "@/types/enums";
import type { AnalyticsTimeSeries } from "@/models/analytics";

// Mock the analytics service
vi.mock("@/api/analyticsService", () => ({
  getAnalyticsTimeSeries: vi.fn(),
}));

import { getAnalyticsTimeSeries } from "@/api/analyticsService";

// Mock CChart component from CoreUI
vi.mock("@coreui/vue-chartjs", () => ({
  CChart: {
    name: "CChart",
    template: "<div class='mock-chart'></div>",
    props: ["type", "data", "options"],
  },
}));

describe("PostsOverTime", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  const mockTimeSeriesData: AnalyticsTimeSeries = {
    period: Period.Day,
    series: [
      {
        bucketStart: new Date("2025-11-30T00:00:00Z"),
        bucketEnd: new Date("2025-11-30T01:00:00Z"),
        postCount: 5,
      },
      {
        bucketStart: new Date("2025-11-30T01:00:00Z"),
        bucketEnd: new Date("2025-11-30T02:00:00Z"),
        postCount: 8,
      },
      {
        bucketStart: new Date("2025-11-30T02:00:00Z"),
        bucketEnd: new Date("2025-11-30T03:00:00Z"),
        postCount: 12,
      },
    ],
  };

  it("renders the component with title", () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    expect(wrapper.find("h4").text()).toBe("Posts Over Time");
  });

  it("renders the chart wrapper", () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    expect(wrapper.find(".chart-wrapper").exists()).toBe(true);
  });

  it("fetches analytics data on mount", async () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    expect(getAnalyticsTimeSeries).toHaveBeenCalledTimes(1);
    expect(getAnalyticsTimeSeries).toHaveBeenCalledWith(Period.Day);
  });

  it("formats labels correctly for Day period (HH:MM format)", async () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toHaveLength(3);
    expect(chartData.labels[0]).toBe("00:00");
    expect(chartData.labels[1]).toBe("01:00");
    expect(chartData.labels[2]).toBe("02:00");
  });

  it("formats labels correctly for Month period (YYYY-MM-DD format)", async () => {
    const monthData: AnalyticsTimeSeries = {
      period: Period.Month,
      series: [
        {
          bucketStart: new Date("2025-11-01T00:00:00Z"),
          bucketEnd: new Date("2025-11-02T00:00:00Z"),
          postCount: 10,
        },
        {
          bucketStart: new Date("2025-11-02T00:00:00Z"),
          bucketEnd: new Date("2025-11-03T00:00:00Z"),
          postCount: 15,
        },
      ],
    };

    getAnalyticsTimeSeries.mockResolvedValue(monthData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Month,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toHaveLength(2);
    expect(chartData.labels[0]).toBe("2025-11-01");
    expect(chartData.labels[1]).toBe("2025-11-02");
  });

  it("formats labels correctly for Year period (YYYY-MM format)", async () => {
    const yearData: AnalyticsTimeSeries = {
      period: Period.Year,
      series: [
        {
          bucketStart: new Date("2025-01-01T00:00:00Z"),
          bucketEnd: new Date("2025-02-01T00:00:00Z"),
          postCount: 20,
        },
        {
          bucketStart: new Date("2025-02-01T00:00:00Z"),
          bucketEnd: new Date("2025-03-01T00:00:00Z"),
          postCount: 25,
        },
      ],
    };

    getAnalyticsTimeSeries.mockResolvedValue(yearData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Year,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toHaveLength(2);
    expect(chartData.labels[0]).toBe("2025-01");
    expect(chartData.labels[1]).toBe("2025-02");
  });

  it("populates chart data with post counts", async () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.datasets).toHaveLength(1);
    expect(chartData.datasets[0].data).toEqual([5, 8, 12]);
    expect(chartData.datasets[0].label).toBe("Posts Created");
  });

  it("refetches data when period prop changes", async () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();
    expect(getAnalyticsTimeSeries).toHaveBeenCalledTimes(1);

    // Change the period
    const monthData: AnalyticsTimeSeries = {
      period: Period.Month,
      series: [
        {
          bucketStart: new Date("2025-11-01T00:00:00Z"),
          bucketEnd: new Date("2025-11-02T00:00:00Z"),
          postCount: 30,
        },
      ],
    };
    getAnalyticsTimeSeries.mockResolvedValue(monthData);

    await wrapper.setProps({ period: Period.Month });
    await flushPromises();

    expect(getAnalyticsTimeSeries).toHaveBeenCalledTimes(2);
    expect(getAnalyticsTimeSeries).toHaveBeenCalledWith(Period.Month);
  });

  it("handles API errors gracefully", async () => {
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    getAnalyticsTimeSeries.mockRejectedValue(new Error("API Error"));

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    expect(consoleErrorSpy).toHaveBeenCalled();
    expect(wrapper.vm.error).toBe("Failed to fetch chart data");
    expect(wrapper.vm.isLoading).toBe(false);

    consoleErrorSpy.mockRestore();
  });

  it("sets loading state during data fetch", async () => {
    let resolvePromise: (value: AnalyticsTimeSeries) => void;
    const promise = new Promise<AnalyticsTimeSeries>((resolve) => {
      resolvePromise = resolve;
    });
    getAnalyticsTimeSeries.mockReturnValue(promise);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    // Should be loading initially
    expect(wrapper.vm.isLoading).toBe(true);

    // Resolve the promise
    resolvePromise!(mockTimeSeriesData);
    await flushPromises();

    // Should no longer be loading
    expect(wrapper.vm.isLoading).toBe(false);
  });

  it("applies correct chart styling and options", async () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.datasets[0].backgroundColor).toBe("rgba(2, 101, 194, 0.1)");
    expect(chartData.datasets[0].borderColor).toBe("rgba(2, 101, 194, 1)");
    expect(chartData.datasets[0].fill).toBe(true);
    expect(chartData.datasets[0].tension).toBe(0.4);
  });

  it("configures chart options correctly", async () => {
    getAnalyticsTimeSeries.mockResolvedValue(mockTimeSeriesData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartOptions = wrapper.vm.chartOptions;
    expect(chartOptions.responsive).toBe(true);
    expect(chartOptions.maintainAspectRatio).toBe(false);
    expect(chartOptions.plugins.legend.display).toBe(false);
    expect(chartOptions.scales.y.beginAtZero).toBe(true);
    expect(chartOptions.scales.y.ticks.precision).toBe(0);
  });

  it("handles empty data gracefully", async () => {
    const emptyData: AnalyticsTimeSeries = {
      period: Period.Day,
      series: [],
    };

    getAnalyticsTimeSeries.mockResolvedValue(emptyData);

    const wrapper = mount(PostsOverTime, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toEqual([]);
    expect(chartData.datasets[0].data).toEqual([]);
  });
});
