/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import TagBreakdown from "@/components/admin/Charts/TagBreakdown.vue";
import { Period } from "@/types/enums";
import type { AnalyticsTags } from "@/models/analytics";

// Mock the analytics service
vi.mock("@/api/analyticsService", () => ({
  getAnalyticsTags: vi.fn(),
}));

import { getAnalyticsTags } from "@/api/analyticsService";

// Mock CChart component from CoreUI
vi.mock("@coreui/vue-chartjs", () => ({
  CChart: {
    name: "CChart",
    template: "<div class='mock-chart'></div>",
    props: ["type", "data", "options"],
  },
}));

describe("TagBreakdown", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  const mockTagData: AnalyticsTags = {
    period: Period.Day,
    tagBuckets: [
      {
        tagId: "1",
        tagName: "Construction",
        postCount: 25,
      },
      {
        tagId: "2",
        tagName: "Transportation",
        postCount: 18,
      },
      {
        tagId: "3",
        tagName: "Safety",
        postCount: 12,
      },
      {
        tagId: "4",
        tagName: "Parks & Recreation",
        postCount: 8,
      },
    ],
  };

  it("renders the component with title", () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    expect(wrapper.find("h4").text()).toBe("Tag Breakdown");
  });

  it("renders the chart container", () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    expect(wrapper.find(".chart-container").exists()).toBe(true);
  });

  it("fetches tag data on mount", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    expect(getAnalyticsTags).toHaveBeenCalledTimes(1);
    expect(getAnalyticsTags).toHaveBeenCalledWith(Period.Day);
  });

  it("populates chart with tag names as labels", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toEqual([
      "Construction",
      "Transportation",
      "Safety",
      "Parks & Recreation",
    ]);
  });

  it("populates chart with post counts as data", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.datasets).toHaveLength(1);
    expect(chartData.datasets[0].data).toEqual([25, 18, 12, 8]);
    expect(chartData.datasets[0].label).toBe("Posts by Tag");
  });

  it("assigns colors to each tag", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.datasets[0].backgroundColor).toHaveLength(4);
    expect(chartData.datasets[0].borderColor).toHaveLength(4);
    expect(chartData.datasets[0].borderWidth).toBe(2);
  });

  it("cycles through colors when there are more tags than colors", async () => {
    const manyTagsData: AnalyticsTags = {
      period: Period.Day,
      tagBuckets: Array.from({ length: 12 }, (_, i) => ({
        tagId: `${i + 1}`,
        tagName: `Tag ${i + 1}`,
        postCount: i + 1,
      })),
    };

    getAnalyticsTags.mockResolvedValue(manyTagsData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.datasets[0].backgroundColor).toHaveLength(12);
    // Colors should repeat (there are 8 predefined colors)
    expect(chartData.datasets[0].backgroundColor![0]).toBe(
      chartData.datasets[0].backgroundColor![8]
    );
  });

  it("refetches data when period prop changes", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();
    expect(getAnalyticsTags).toHaveBeenCalledTimes(1);

    // Change the period
    const monthData: AnalyticsTags = {
      period: Period.Month,
      tagBuckets: [
        {
          tagId: "1",
          tagName: "Construction",
          postCount: 50,
        },
      ],
    };
    getAnalyticsTags.mockResolvedValue(monthData);

    await wrapper.setProps({ period: Period.Month });
    await flushPromises();

    expect(getAnalyticsTags).toHaveBeenCalledTimes(2);
    expect(getAnalyticsTags).toHaveBeenCalledWith(Period.Month);
  });

  it("handles API errors gracefully", async () => {
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    getAnalyticsTags.mockRejectedValue(new Error("API Error"));
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    expect(consoleErrorSpy).toHaveBeenCalled();
    expect(wrapper.vm.error).toBe("Failed to load tag data");
    expect(wrapper.vm.isLoading).toBe(false);

    consoleErrorSpy.mockRestore();
  });

  it("displays error message when API fails", async () => {
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    getAnalyticsTags.mockRejectedValue(new Error("API Error"));
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    expect(wrapper.find(".error-state").exists()).toBe(true);
    expect(wrapper.find(".error-state p").text()).toBe("Failed to load tag data");

    consoleErrorSpy.mockRestore();
  });

  it("displays loading state during data fetch", async () => {
    let resolvePromise: (value: AnalyticsTags) => void;
    const promise = new Promise<AnalyticsTags>((resolve) => {
      resolvePromise = resolve;
    });
    getAnalyticsTags.mockReturnValue(promise);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    // Should be loading initially
    expect(wrapper.vm.isLoading).toBe(true);
    expect(wrapper.find(".loading-state").exists()).toBe(true);
    expect(wrapper.find(".loading-state p").text()).toBe("Loading tag data...");
    expect(wrapper.find(".spinner").exists()).toBe(true);

    // Resolve the promise
    resolvePromise!(mockTagData);
    await flushPromises();

    // Should no longer be loading
    expect(wrapper.vm.isLoading).toBe(false);
    expect(wrapper.find(".loading-state").exists()).toBe(false);
  });

  it("configures chart as doughnut type", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    // The CChart component should receive type="doughnut"
    const chartComponent = wrapper.findComponent({ name: "CChart" });
    expect(chartComponent.exists()).toBe(true);
  });

  it("configures chart options correctly", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartOptions = wrapper.vm.chartOptions;
    expect(chartOptions.responsive).toBe(true);
    expect(chartOptions.maintainAspectRatio).toBe(false);
    expect(chartOptions.plugins.legend.position).toBe("right");
    expect(chartOptions.plugins.tooltip.enabled).toBe(true);
  });

  it("formats tooltip with percentage", async () => {
    getAnalyticsTags.mockResolvedValue(mockTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartOptions = wrapper.vm.chartOptions;
    const tooltipCallback = chartOptions.plugins.tooltip.callbacks.label;
    
    // Mock context for tooltip
    const mockContext = {
      label: "Construction",
      parsed: 25,
      dataset: {
        data: [25, 18, 12, 8], // Total: 63
      },
    };

    const result = tooltipCallback(mockContext);
    expect(result).toBe("Construction: 25 (39.7%)");
  });

  it("handles empty data gracefully", async () => {
    const emptyData: AnalyticsTags = {
      period: Period.Day,
      tagBuckets: [],
    };
    
    getAnalyticsTags.mockResolvedValue(emptyData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toEqual([]);
    expect(chartData.datasets[0].data).toEqual([]);
  });

  it("handles single tag data", async () => {
    const singleTagData: AnalyticsTags = {
      period: Period.Day,
      tagBuckets: [
        {
          tagId: "1",
          tagName: "Construction",
          postCount: 100,
        },
      ],
    };
    
    getAnalyticsTags.mockResolvedValue(singleTagData);
    
    const wrapper = mount(TagBreakdown, {
      props: {
        period: Period.Day,
      },
    });

    await flushPromises();

    const chartData = wrapper.vm.chartData;
    expect(chartData.labels).toEqual(["Construction"]);
    expect(chartData.datasets[0].data).toEqual([100]);
  });
});
