/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import PostActivity from "@/components/admin/PostActivity.vue";
import { Period } from "@/types/enums";
import type { AnalyticsSummary } from "@/models/analytics";

// Mock the analytics service
vi.mock("@/api/analyticsService", () => ({
  getAnalyticsSummary: vi.fn(),
}));

import { getAnalyticsSummary } from "@/api/analyticsService";

// Mock child components
vi.mock("@/components/admin/Charts/PostsOverTime.vue", () => ({
  default: {
    name: "PostsOverTime",
    template: "<div class='mock-posts-over-time'></div>",
    props: ["period"],
  },
}));

vi.mock("@/components/admin/Charts/TagBreakdown.vue", () => ({
  default: {
    name: "TagBreakdown",
    template: "<div class='mock-tag-breakdown'></div>",
    props: ["period"],
  },
}));

describe("PostActivity", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  const mockSummaryData: AnalyticsSummary = {
    period: Period.Day,
    startDate: new Date("2025-11-30T00:00:00Z"),
    endDate: new Date("2025-12-01T00:00:00Z"),
    totalPosts: 42,
    totalUpvotes: 150,
    totalDownvotes: 25,
    totalComments: 88,
  };

  it("renders the component with title", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    expect(wrapper.find("h3").text()).toBe("Activity");
  });

  it("renders all three period buttons", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    const buttons = wrapper.findAll("button");
    expect(buttons.length).toBeGreaterThanOrEqual(3);
    
    const buttonTexts = buttons.map(btn => btn.text());
    expect(buttonTexts).toContain("Day");
    expect(buttonTexts).toContain("Month");
    expect(buttonTexts).toContain("Year");
  });

  it("has Day period selected by default", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    expect(wrapper.vm.selectedPeriod).toBe(Period.Day);
  });

  it("renders all four stat cards", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    expect(wrapper.find(".stat-card.new-posts").exists()).toBe(true);
    expect(wrapper.find(".stat-card.upvotes").exists()).toBe(true);
    expect(wrapper.find(".stat-card.downvotes").exists()).toBe(true);
    expect(wrapper.find(".stat-card.comments").exists()).toBe(true);
  });

  it("fetches analytics summary on mount", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    mount(PostActivity);

    await flushPromises();

    expect(getAnalyticsSummary).toHaveBeenCalledTimes(1);
    expect(getAnalyticsSummary).toHaveBeenCalledWith(Period.Day);
  });

  it("displays correct stat values", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    await flushPromises();

    const statValues = wrapper.findAll(".stat-value");
    expect(statValues[0].text()).toBe("42");
    expect(statValues[1].text()).toBe("150");
    expect(statValues[2].text()).toBe("25");
    expect(statValues[3].text()).toBe("88");
  });

  it("displays correct stat labels", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    await flushPromises();

    const statLabels = wrapper.findAll(".stat-label");
    expect(statLabels[0].text()).toBe("New Posts");
    expect(statLabels[1].text()).toBe("Upvotes");
    expect(statLabels[2].text()).toBe("Downvotes");
    expect(statLabels[3].text()).toBe("Comments");
  });

  it("changes period when Day button is clicked", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    // Set to a different period first
    wrapper.vm.selectedPeriod = Period.Month;
    await wrapper.vm.$nextTick();

    const buttons = wrapper.findAll("button");
    const dayButton = buttons.find(btn => btn.text() === "Day");
    await dayButton!.trigger("click");

    expect(wrapper.vm.selectedPeriod).toBe(Period.Day);
  });

  it("changes period when Month button is clicked", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    const buttons = wrapper.findAll("button");
    const monthButton = buttons.find(btn => btn.text() === "Month");
    await monthButton!.trigger("click");

    expect(wrapper.vm.selectedPeriod).toBe(Period.Month);
  });

  it("changes period when Year button is clicked", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    const buttons = wrapper.findAll("button");
    const yearButton = buttons.find(btn => btn.text() === "Year");
    await yearButton!.trigger("click");

    expect(wrapper.vm.selectedPeriod).toBe(Period.Year);
  });

  it("refetches stats when period changes", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    expect(getAnalyticsSummary).toHaveBeenCalledTimes(1);

    const monthData: AnalyticsSummary = {
      period: Period.Month,
      startDate: new Date("2025-11-01T00:00:00Z"),
      endDate: new Date("2025-12-01T00:00:00Z"),
      totalPosts: 500,
      totalUpvotes: 2000,
      totalDownvotes: 300,
      totalComments: 1200,
    };
    getAnalyticsSummary.mockResolvedValue(monthData);

    const buttons = wrapper.findAll("button");
    const monthButton = buttons.find(btn => btn.text() === "Month");
    await monthButton!.trigger("click");
    await flushPromises();

    expect(getAnalyticsSummary).toHaveBeenCalledTimes(2);
    expect(getAnalyticsSummary).toHaveBeenCalledWith(Period.Month);
  });

  it("updates stat values when period changes", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    let statValues = wrapper.findAll(".stat-value");
    expect(statValues[0].text()).toBe("42");

    const monthData: AnalyticsSummary = {
      period: Period.Month,
      startDate: new Date("2025-11-01T00:00:00Z"),
      endDate: new Date("2025-12-01T00:00:00Z"),
      totalPosts: 500,
      totalUpvotes: 2000,
      totalDownvotes: 300,
      totalComments: 1200,
    };
    getAnalyticsSummary.mockResolvedValue(monthData);

    const buttons = wrapper.findAll("button");
    const monthButton = buttons.find(btn => btn.text() === "Month");
    await monthButton!.trigger("click");
    await flushPromises();

    statValues = wrapper.findAll(".stat-value");
    expect(statValues[0].text()).toBe("500");
    expect(statValues[1].text()).toBe("2000");
    expect(statValues[2].text()).toBe("300");
    expect(statValues[3].text()).toBe("1200");
  });

  it("renders PostsOverTime component", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    expect(wrapper.findComponent({ name: "PostsOverTime" }).exists()).toBe(true);
  });

  it("renders TagBreakdown component", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    expect(wrapper.findComponent({ name: "TagBreakdown" }).exists()).toBe(true);
  });

  it("passes selected period to PostsOverTime component", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    let postsOverTime = wrapper.findComponent({ name: "PostsOverTime" });
    expect(postsOverTime.props("period")).toBe(Period.Day);

    const buttons = wrapper.findAll("button");
    const monthButton = buttons.find(btn => btn.text() === "Month");
    await monthButton!.trigger("click");
    await wrapper.vm.$nextTick();

    postsOverTime = wrapper.findComponent({ name: "PostsOverTime" });
    expect(postsOverTime.props("period")).toBe(Period.Month);
  });

  it("passes selected period to TagBreakdown component", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    let tagBreakdown = wrapper.findComponent({ name: "TagBreakdown" });
    expect(tagBreakdown.props("period")).toBe(Period.Day);

    const buttons = wrapper.findAll("button");
    const yearButton = buttons.find(btn => btn.text() === "Year");
    await yearButton!.trigger("click");
    await wrapper.vm.$nextTick();

    tagBreakdown = wrapper.findComponent({ name: "TagBreakdown" });
    expect(tagBreakdown.props("period")).toBe(Period.Year);
  });

  it("handles API errors gracefully", async () => {
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    getAnalyticsSummary.mockRejectedValue(new Error("API Error"));
    
    const wrapper = mount(PostActivity);

    await flushPromises();

    expect(consoleErrorSpy).toHaveBeenCalled();
    expect(wrapper.vm.error).toBe("Failed to fetch stats");
    expect(wrapper.vm.isLoading).toBe(false);

    consoleErrorSpy.mockRestore();
  });

  it("sets loading state during data fetch", async () => {
    let resolvePromise: (value: AnalyticsSummary) => void;
    const promise = new Promise<AnalyticsSummary>((resolve) => {
      resolvePromise = resolve;
    });
    getAnalyticsSummary.mockReturnValue(promise);
    
    const wrapper = mount(PostActivity);

    // Should be loading initially
    expect(wrapper.vm.isLoading).toBe(true);

    // Resolve the promise
    resolvePromise!(mockSummaryData);
    await flushPromises();

    // Should no longer be loading
    expect(wrapper.vm.isLoading).toBe(false);
  });

  it("initializes stats with zero values", () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);

    // Before data loads, stats should be 0
    expect(wrapper.vm.stats.newPosts).toBe(0);
    expect(wrapper.vm.stats.upvotes).toBe(0);
    expect(wrapper.vm.stats.downvotes).toBe(0);
    expect(wrapper.vm.stats.comments).toBe(0);
  });

  it("applies active class to selected period button", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    const buttons = wrapper.findAll("button");
    const dayButton = buttons.find(btn => btn.text() === "Day");
    
    expect(dayButton!.classes()).toContain("active");
  });

  it("updates active class when period changes", async () => {
    getAnalyticsSummary.mockResolvedValue(mockSummaryData);
    
    const wrapper = mount(PostActivity);
    await flushPromises();

    const buttons = wrapper.findAll("button");
    const monthButton = buttons.find(btn => btn.text() === "Month");
    await monthButton!.trigger("click");
    await wrapper.vm.$nextTick();

    expect(monthButton!.classes()).toContain("active");
  });
});
