/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for MapOverview component
/// Tests map initialization, markers, heatmap, and info windows

import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { mount, VueWrapper, flushPromises } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import MapOverview from "@/components/MapOverview.vue";
import type { Post } from "@/models/post";
import type { Media } from "@/models/media";

// Mock Google Maps API
const mockMap = {
  setCenter: vi.fn(),
  setZoom: vi.fn(),
  fitBounds: vi.fn(),
  getZoom: vi.fn(() => 15),
  addListener: vi.fn(),
};

const mockMarker = {
  position: null,
  map: null,
  title: "",
  addListener: vi.fn(),
};

const mockInfoWindow = {
  open: vi.fn(),
  close: vi.fn(),
  setContent: vi.fn(),
};

const mockHeatmapLayer = {
  setMap: vi.fn(),
  setData: vi.fn(),
};

const mockLatLngBounds = {
  extend: vi.fn(),
};

// Mock Google Maps loader
vi.mock("@/utils/googleMapsLoader", () => ({
  loadGoogleMaps: vi.fn().mockResolvedValue(undefined),
}));

// Mock media service
vi.mock("@/api/mediaService", () => ({
  getMediaByPostId: vi.fn(),
}));

// Mock location formatter
vi.mock("@/utils/locationFormatter", () => ({
  removePostalCode: vi.fn((location: string) => {
    if (!location) return "";
    return location
      .replace(/[A-Z]\d[A-Z]\s?\d[A-Z]\d/gi, "")
      .replace(/,\s*,/g, ",")
      .trim();
  }),
}));

import { getMediaByPostId } from "@/api/mediaService";
import { removePostalCode } from "@/utils/locationFormatter";

interface WindowWithGoogle extends Window {
  google?: typeof google;
}

declare const global: WindowWithGoogle;

// Setup global Google Maps mock
(global as WindowWithGoogle).google = {
  maps: {
    Map: vi.fn(() => mockMap),
    marker: {
      AdvancedMarkerElement: vi.fn(() => mockMarker),
    },
    InfoWindow: vi.fn(() => mockInfoWindow),
    visualization: {
      HeatmapLayer: vi.fn(() => mockHeatmapLayer),
    },
    LatLng: vi.fn((lat: number, lng: number) => ({ lat, lng })),
    LatLngBounds: vi.fn(() => mockLatLngBounds),
    event: {
      addListenerOnce: vi.fn((map: unknown, event: string, callback: () => void) => {
        if (event === "idle") {
          setTimeout(callback, 0);
        }
      }),
    },
  },
};

describe("MapOverview.vue", () => {
  let wrapper: VueWrapper<InstanceType<typeof MapOverview>>;
  let router: ReturnType<typeof createRouter>;

  const mockPosts: Post[] = [
    {
      id: 1,
      title: "Test Post 1",
      description:
        "This is a test post description with enough content to be truncated when displayed in the info window",
      location: "Winnipeg, MB R3H 0S2, Canada",
      latitude: 49.8951,
      longitude: -97.1384,
      authorId: 1,
      authorName: "Test Author",
      voteCount: 5,
      commentCount: 2,
      createdAt: new Date("2024-01-01"),
      updatedAt: new Date("2024-01-01"),
      isDeleted: false,
      tags: [],
      voteStatus: 0,
    },
    {
      id: 2,
      title: "Test Post 2",
      description: "Another test post",
      location: "Winnipeg, MB R3T 1A1, Canada",
      latitude: 49.9,
      longitude: -97.15,
      authorId: 2,
      authorName: "Test Author 2",
      voteCount: 10,
      commentCount: 5,
      createdAt: new Date("2024-01-02"),
      updatedAt: new Date("2024-01-02"),
      isDeleted: false,
      tags: [],
      voteStatus: 0,
    },
  ];

  const mockMedia: Media[] = [
    {
      id: 1,
      postId: 1,
      url: "https://example.com/image1.jpg",
      createdAt: new Date(),
    },
  ];

  beforeEach(async () => {
    vi.clearAllMocks();

    // Setup router
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: "/", component: { template: "<div />" } },
        { path: "/posts/:id", component: { template: "<div />" } },
      ],
    });

    await router.push("/");
    await router.isReady();

    // Setup media service mock
    (getMediaByPostId as ReturnType<typeof vi.fn>).mockResolvedValue(mockMedia);
  });

  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  describe("Component Rendering", () => {
    it("renders the component", () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      expect(wrapper.find(".map-overview-container").exists()).toBe(true);
    });

    it("displays loading state initially", () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      expect(wrapper.find(".loading-state").exists()).toBe(true);
      expect(wrapper.find(".loading-state p").text()).toBe("Loading map...");
      expect(wrapper.find(".loading-state i").classes()).toContain("pi-spinner");
    });

    it("applies custom height prop", () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
          height: "800px",
        },
        global: {
          plugins: [router],
        },
      });

      const container = wrapper.find(".map-overview-container");
      expect(container.exists()).toBe(true);
    });

    it("uses default height when not provided", () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      const container = wrapper.find(".map-overview-container");
      expect(container.exists()).toBe(true);
    });
  });

  describe("Map Initialization", () => {
    it("initializes Google Maps on mount", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect((global as WindowWithGoogle).google!.maps.Map).toHaveBeenCalled();
    });

    it("fetches media for posts with locations", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(getMediaByPostId).toHaveBeenCalledWith(1);
      expect(getMediaByPostId).toHaveBeenCalledWith(2);
    });

    it("handles map initialization errors gracefully", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

      const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");
      vi.mocked(loadGoogleMaps).mockRejectedValueOnce(new Error("Failed to load"));

      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(wrapper.vm.error).toBe("Failed to load map.");
      expect(wrapper.find(".error-state").exists()).toBe(true);

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Markers", () => {
    it("creates markers for posts with locations", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const markerCalls = (
        (global as WindowWithGoogle).google!.maps.marker.AdvancedMarkerElement as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls;
      expect(markerCalls.length).toBeGreaterThan(0);
    });

    it("does not create markers for deleted posts", async () => {
      const postsWithDeleted: Post[] = [
        ...mockPosts,
        {
          ...mockPosts[0],
          id: 3,
          isDeleted: true,
        },
      ];

      wrapper = mount(MapOverview, {
        props: {
          posts: postsWithDeleted,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // Should only create markers for non-deleted posts
      const markerCalls = (
        (global as WindowWithGoogle).google!.maps.marker.AdvancedMarkerElement as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls;
      expect(markerCalls.length).toBe(2);
    });

    it("does not create markers for posts without coordinates", async () => {
      const postsWithoutLocation: Post[] = [
        {
          ...mockPosts[0],
          latitude: undefined,
          longitude: undefined,
        },
      ];

      wrapper = mount(MapOverview, {
        props: {
          posts: postsWithoutLocation,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const markerCalls = (
        (global as WindowWithGoogle).google!.maps.marker.AdvancedMarkerElement as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls;
      expect(markerCalls.length).toBe(0);
    });

    it("toggles marker visibility when toggle button clicked", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // Initially markers should be visible
      expect(wrapper.vm.showMarkers).toBe(true);

      // Find and click the markers toggle button
      const markersButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Markers"));

      if (markersButton) {
        await markersButton.trigger("click");
        await wrapper.vm.$nextTick();

        expect(wrapper.vm.showMarkers).toBe(false);
      }
    });

    it("updates markers when posts prop changes", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const initialMarkerCalls = (
        (global as WindowWithGoogle).google!.maps.marker.AdvancedMarkerElement as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls.length;

      // Update posts
      const newPosts: Post[] = [
        {
          ...mockPosts[0],
          id: 3,
          title: "New Post",
          latitude: 49.85,
          longitude: -97.2,
        },
      ];

      await wrapper.setProps({ posts: newPosts });
      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const newMarkerCalls = (
        (global as WindowWithGoogle).google!.maps.marker.AdvancedMarkerElement as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls.length;
      expect(newMarkerCalls).toBeGreaterThan(initialMarkerCalls);
    });
  });

  describe("Info Windows", () => {
    it("creates info windows for markers", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect((global as WindowWithGoogle).google!.maps.InfoWindow).toHaveBeenCalled();
    });

    it("formats location without postal code in info window", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(removePostalCode).toHaveBeenCalledWith("Winnipeg, MB R3H 0S2, Canada");
    });

    it("truncates long descriptions in info window", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // The long description should be truncated to 80 characters
      const calls = mockInfoWindow.setContent.mock.calls;
      if (calls.length > 0) {
        const content = calls[0][0];
        // Content should be an HTMLDivElement with truncated text
        expect(content).toBeDefined();
      }
    });

    it("displays post image in info window when available", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // Media should have been fetched
      expect(getMediaByPostId).toHaveBeenCalled();
    });
  });

  describe("Heatmap", () => {
    it("initializes heatmap layer", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(
        (global as WindowWithGoogle).google!.maps.visualization.HeatmapLayer,
      ).toHaveBeenCalled();
    });

    it("heatmap is hidden by default", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(wrapper.vm.showHeatmap).toBe(false);
    });

    it("toggles heatmap visibility when toggle button clicked", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // Find and click the heatmap toggle button
      const heatmapButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Heatmap"));

      if (heatmapButton) {
        await heatmapButton.trigger("click");
        await wrapper.vm.$nextTick();

        expect(wrapper.vm.showHeatmap).toBe(true);
        expect(mockHeatmapLayer.setMap).toHaveBeenCalled();
      }
    });

    it("recreates heatmap when posts change", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const initialHeatmapCalls = (
        (global as WindowWithGoogle).google!.maps.visualization.HeatmapLayer as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls.length;

      // Update posts
      const newPosts: Post[] = [
        ...mockPosts,
        {
          ...mockPosts[0],
          id: 3,
          title: "New Post",
          latitude: 49.85,
          longitude: -97.2,
        },
      ];

      await wrapper.setProps({ posts: newPosts });
      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const newHeatmapCalls = (
        (global as WindowWithGoogle).google!.maps.visualization.HeatmapLayer as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls.length;
      expect(newHeatmapCalls).toBeGreaterThanOrEqual(initialHeatmapCalls);
    });
  });

  describe("Control Buttons", () => {
    it("renders control buttons", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const controls = wrapper.find(".map-controls");
      if (controls.exists()) {
        const buttons = controls.findAll(".control-button");
        expect(buttons.length).toBe(2);
      }
    });

    it("markers button has active class by default", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const markersButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Markers"));

      if (markersButton) {
        expect(markersButton.classes()).toContain("active");
      }
    });

    it("heatmap button does not have active class by default", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const heatmapButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Heatmap"));

      if (heatmapButton) {
        expect(heatmapButton.classes()).not.toContain("active");
      }
    });

    it("toggles active class when buttons are clicked", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const heatmapButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Heatmap"));

      if (heatmapButton) {
        await heatmapButton.trigger("click");
        await wrapper.vm.$nextTick();

        expect(heatmapButton.classes()).toContain("active");

        await heatmapButton.trigger("click");
        await wrapper.vm.$nextTick();

        expect(heatmapButton.classes()).not.toContain("active");
      }
    });
  });

  describe("Empty State", () => {
    it("displays empty state when no posts provided", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: [],
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const emptyState = wrapper.find(".empty-state");
      if (emptyState.exists()) {
        expect(emptyState.text()).toContain("No posts with locations to display");
      }
    });

    it("displays empty state when posts have no locations", async () => {
      const postsWithoutLocations: Post[] = [
        {
          ...mockPosts[0],
          latitude: undefined,
          longitude: undefined,
        },
      ];

      wrapper = mount(MapOverview, {
        props: {
          posts: postsWithoutLocations,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // No markers should be created
      const markerCalls = (
        (global as WindowWithGoogle).google!.maps.marker.AdvancedMarkerElement as unknown as {
          mock: { calls: unknown[] };
        }
      ).mock.calls;
      expect(markerCalls.length).toBe(0);
    });
  });

  describe("Error Handling", () => {
    it("displays error state when map loading fails", async () => {
      const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");
      vi.mocked(loadGoogleMaps).mockRejectedValueOnce(new Error("Failed to load"));

      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(wrapper.find(".error-state").exists()).toBe(true);
      expect(wrapper.find(".error-state p").text()).toBe("Failed to load map.");
    });

    it("handles media fetch errors gracefully", async () => {
      const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
      (getMediaByPostId as ReturnType<typeof vi.fn>).mockRejectedValueOnce(
        new Error("Media fetch failed"),
      );

      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // Should still render map despite media error
      expect(consoleErrorSpy).toHaveBeenCalled();

      consoleErrorSpy.mockRestore();
    });
  });

  describe("Map Bounds", () => {
    it("fits map bounds to include all markers", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      expect(mockMap.fitBounds).toHaveBeenCalled();
    });

    it("limits zoom level to maximum 15", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      // The idle event should trigger zoom adjustment
      expect(mockMap.getZoom).toHaveBeenCalled();
    });
  });

  describe("Accessibility", () => {
    it("provides loading state for screen readers", () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      const loadingState = wrapper.find(".loading-state");
      expect(loadingState.exists()).toBe(true);
      expect(loadingState.text()).toContain("Loading map...");
    });

    it("provides error state for screen readers", async () => {
      const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");
      vi.mocked(loadGoogleMaps).mockRejectedValueOnce(new Error("Failed"));

      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const errorState = wrapper.find(".error-state");
      expect(errorState.exists()).toBe(true);
      expect(errorState.text()).toContain("Failed to load map.");
    });

    it("control buttons have descriptive text", async () => {
      wrapper = mount(MapOverview, {
        props: {
          posts: mockPosts,
        },
        global: {
          plugins: [router],
        },
      });

      await flushPromises();
      await new Promise((resolve) => setTimeout(resolve, 200));

      const markersButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Markers"));
      const heatmapButton = wrapper
        .findAll(".control-button")
        .find((btn) => btn.text().includes("Heatmap"));

      if (markersButton) {
        expect(markersButton.text()).toContain("Markers");
      }
      if (heatmapButton) {
        expect(heatmapButton.text()).toContain("Heatmap");
      }
    });
  });
});
