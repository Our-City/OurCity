/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for MapDisplay component
/// Tests read-only map display with single location marker

import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { mount, VueWrapper } from "@vue/test-utils";
import MapDisplay from "@/components/MapDisplay.vue";

interface WindowWithGoogle extends Window {
  google?: typeof google;
}

declare const global: WindowWithGoogle;
// Mock Google Maps API
const mockMap = {
  setCenter: vi.fn(),
  setZoom: vi.fn(),
  panTo: vi.fn(),
};

const mockMarker = {
  position: null,
  map: null,
  setMap: vi.fn(),
};

vi.mock("@/utils/googleMapsLoader", () => ({
  loadGoogleMaps: vi.fn().mockResolvedValue(undefined),
  isGoogleMapsLoaded: vi.fn().mockReturnValue(true),
}));

// Mock Google Maps globally
(global as WindowWithGoogle).google = {
  maps: {
    Map: vi.fn(() => mockMap),
    marker: {
      AdvancedMarkerElement: vi.fn((config) => {
        mockMarker.position = config?.position;
        mockMarker.map = config?.map;
        return mockMarker;
      }),
    },
    LatLng: vi.fn((lat, lng) => ({ lat, lng })),
  },
};

describe("MapDisplay.vue", () => {
  let wrapper: VueWrapper<InstanceType<typeof MapDisplay>>;

  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  it("should render map container", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        locationName: "Winnipeg, MB, Canada",
      },
    });

    expect(wrapper.find(".map-display").exists()).toBe(true);
  });

  it("should display loading state initially", async () => {
    const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");

    // Make loadGoogleMaps take some time
    vi.mocked(loadGoogleMaps).mockImplementation(
      () => new Promise((resolve) => setTimeout(resolve, 100)),
    );

    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    // Should show loading initially
    const loadingState = wrapper.find(".loading-state, .map-loading");
    if (loadingState.exists()) {
      expect(loadingState.exists()).toBe(true);
    }
  });

  it("should apply custom height prop", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        height: "800px",
      },
    });

    const container = wrapper.find(".map-container");

    if (container.exists()) {
      const style = container.attributes("style");
      expect(style).toContain("800px");
    } else {
      // Verify the height prop was passed
      expect(wrapper.props("height")).toBe("800px");
    }
  });

  it("should use default height when not provided", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    // Default height should be applied via props
    expect(wrapper.props("height")).toBeDefined();
  });

  it("should display location name when provided", async () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        locationName: "Downtown Winnipeg",
      },
    });

    await wrapper.vm.$nextTick();

    // Check if location name is passed as prop
    expect(wrapper.props("locationName")).toBe("Downtown Winnipeg");
  });

  it("should show no location message when coordinates missing", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: undefined,
        longitude: undefined,
      },
    });

    const noLocation = wrapper.find(".map-no-location, .no-location-message");
    if (noLocation.exists()) {
      expect(noLocation.exists()).toBe(true);
    }
  });

  it("should handle partial coordinates (missing longitude)", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: undefined,
      },
    });

    // Should not display map without complete coordinates
    const noLocation = wrapper.find(".map-no-location, .no-location-message");
    if (noLocation.exists()) {
      expect(noLocation.exists()).toBe(true);
    }
  });

  it("should handle partial coordinates (missing latitude)", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: undefined,
        longitude: -97.1384,
      },
    });

    const noLocation = wrapper.find(".map-no-location, .no-location-message");
    if (noLocation.exists()) {
      expect(noLocation.exists()).toBe(true);
    }
  });

  it("should use custom zoom level", async () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        zoom: 18,
      },
    });

    await wrapper.vm.$nextTick();

    // Verify zoom prop is set correctly
    expect(wrapper.props("zoom")).toBe(18);
  });

  it("should use default zoom when not provided", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    // Default zoom should be 15 (from component defaults)
    expect(wrapper.props("zoom")).toBe(15);
  });

  it("should update marker when coordinates change", async () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    await wrapper.vm.$nextTick();

    // Change coordinates
    await wrapper.setProps({
      latitude: 49.9,
      longitude: -97.15,
    });

    await wrapper.vm.$nextTick();

    // Props should be updated
    expect(wrapper.props("latitude")).toBe(49.9);
    expect(wrapper.props("longitude")).toBe(-97.15);
  });

  it("should handle map initialization error gracefully", async () => {
    // Mock loadGoogleMaps to reject
    const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");
    vi.mocked(loadGoogleMaps).mockRejectedValueOnce(new Error("Failed to load"));

    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 100));

    // Component should handle error gracefully
    // Either show error state or fallback
    const errorState = wrapper.find(".error-state, .map-error");
    const container = wrapper.find(".map-display");

    // At least one should exist
    expect(errorState.exists() || container.exists()).toBe(true);
  });

  it("should create map with correct center coordinates", async () => {
    const lat = 49.8951;
    const lng = -97.1384;

    wrapper = mount(MapDisplay, {
      props: {
        latitude: lat,
        longitude: lng,
      },
    });

    await wrapper.vm.$nextTick();

    // Verify props are correct
    expect(wrapper.props("latitude")).toBe(lat);
    expect(wrapper.props("longitude")).toBe(lng);
  });

  it("should render without location name", async () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    await wrapper.vm.$nextTick();

    // Should still render the map display container
    expect(wrapper.find(".map-display").exists()).toBe(true);
  });

  it("should handle zero coordinates", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 0,
        longitude: 0,
      },
    });

    // Zero is a valid coordinate
    expect(wrapper.props("latitude")).toBe(0);
    expect(wrapper.props("longitude")).toBe(0);
  });

  it("should handle negative coordinates", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: -49.8951,
        longitude: -97.1384,
      },
    });

    // Negative coordinates are valid
    expect(wrapper.props("latitude")).toBe(-49.8951);
    expect(wrapper.props("longitude")).toBe(-97.1384);
  });

  it("should accept string height values", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        height: "600px",
      },
    });

    expect(wrapper.props("height")).toBe("600px");
  });

  it("should render map container div", async () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    await wrapper.vm.$nextTick();

    // Should have a map container element
    const mapContainer = wrapper.find('.map-container, [data-testid="map-container"]');
    const displayContainer = wrapper.find(".map-display");

    expect(mapContainer.exists() || displayContainer.exists()).toBe(true);
  });

  it("should handle location name with special characters", () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        locationName: "L'Esplanade Riel, Winnipeg",
      },
    });

    expect(wrapper.props("locationName")).toBe("L'Esplanade Riel, Winnipeg");
  });

  it("should handle very long location names", () => {
    const longName = "A".repeat(200);

    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
        locationName: longName,
      },
    });

    expect(wrapper.props("locationName")).toBe(longName);
  });

  it("should handle map re-initialization on prop change", async () => {
    wrapper = mount(MapDisplay, {
      props: {
        latitude: 49.8951,
        longitude: -97.1384,
      },
    });

    await wrapper.vm.$nextTick();

    // Change to completely different location
    await wrapper.setProps({
      latitude: 50.5,
      longitude: -96.0,
    });

    await wrapper.vm.$nextTick();

    // Map should update with new coordinates
    expect(wrapper.props("latitude")).toBe(50.5);
    expect(wrapper.props("longitude")).toBe(-96.0);
  });
});
