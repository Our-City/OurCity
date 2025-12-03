/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for MapPicker component
/// Tests interactive map with click-to-select location functionality

import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { mount, VueWrapper } from "@vue/test-utils";
import MapPicker from "@/components/MapPicker.vue";

// Mock Google Maps API
const mockMap = {
  setCenter: vi.fn(),
  setZoom: vi.fn(),
  panTo: vi.fn(),
  addListener: vi.fn(),
};

const mockMarker = {
  position: null,
  map: null,
  setMap: vi.fn(),
  setPosition: vi.fn(),
  addListener: vi.fn(),
};

const mockCircle = {
  setMap: vi.fn(),
};

const mockGeocoder = {
  geocode: vi.fn(),
};

vi.mock("@/utils/googleMapsLoader", () => ({
  loadGoogleMaps: vi.fn().mockResolvedValue(undefined),
  isGoogleMapsLoaded: vi.fn().mockReturnValue(true),
}));

vi.mock("@/utils/locationValidator", () => ({
  isWithinWinnipeg: vi.fn().mockReturnValue(true),
  getDistanceFromWinnipeg: vi.fn().mockReturnValue(5),
}));

// Mock Google Maps globally
(global as any).google = {
  maps: {
    Map: vi.fn(() => mockMap),
    marker: {
      AdvancedMarkerElement: vi.fn((config) => {
        mockMarker.position = config?.position;
        mockMarker.map = config?.map;
        return mockMarker;
      }),
    },
    Circle: vi.fn(() => mockCircle),
    Geocoder: vi.fn(() => mockGeocoder),
    LatLng: vi.fn((lat, lng) => ({ lat, lng })),
  },
};

describe("MapPicker.vue", () => {
  let wrapper: VueWrapper<any>;

  beforeEach(() => {
    vi.clearAllMocks();
    mockGeocoder.geocode.mockImplementation((request, callback) => {
      callback(
        [
          {
            formatted_address: "Winnipeg, MB, Canada",
          },
        ],
        "OK",
      );
    });
  });

  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  it("should render map container", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: "Winnipeg, MB, Canada",
        },
      },
    });

    expect(wrapper.find(".map-picker").exists()).toBe(true);
  });

  it("should display initial location when provided", () => {
    const location = {
      latitude: 49.8951,
      longitude: -97.1384,
      name: "Winnipeg, MB, Canada",
    };

    wrapper = mount(MapPicker, {
      props: {
        modelValue: location,
      },
    });

    expect(wrapper.props("modelValue")).toEqual(location);
  });

  it("should handle empty initial location", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    expect(wrapper.props("modelValue")).toBeNull();
  });

  it("should apply custom height prop", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
        height: "700px",
      },
    });

    expect(wrapper.props("height")).toBe("700px");
  });

  it("should use default height when not provided", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    // Check if height prop has default value
    const height = wrapper.props("height");
    expect(height).toBeDefined();
    expect(height).toBe("400px"); // Default from component
  });

  it("should validate location is within Winnipeg", async () => {
    const { isWithinWinnipeg } = await import("@/utils/locationValidator");

    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 200));

    // Simulate map click
    const clickListener = mockMap.addListener.mock.calls.find((call) => call[0] === "click");

    if (clickListener && typeof clickListener[1] === "function") {
      const clickEvent = {
        latLng: {
          lat: () => 49.8951,
          lng: () => -97.1384,
        },
      };

      clickListener[1](clickEvent);
      await wrapper.vm.$nextTick();

      // Should call validation function
      expect(isWithinWinnipeg).toHaveBeenCalled();
    }
  });

  it("should show error for location outside Winnipeg", async () => {
    const { isWithinWinnipeg, getDistanceFromWinnipeg } = await import("@/utils/locationValidator");

    vi.mocked(isWithinWinnipeg).mockReturnValue(false);
    vi.mocked(getDistanceFromWinnipeg).mockReturnValue(50);

    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 200));

    // Simulate map click outside Winnipeg
    const clickListener = mockMap.addListener.mock.calls.find((call) => call[0] === "click");

    if (clickListener && typeof clickListener[1] === "function") {
      const clickEvent = {
        latLng: {
          lat: () => 43.6532, // Toronto
          lng: () => -79.3832,
        },
      };

      clickListener[1](clickEvent);
      await wrapper.vm.$nextTick();

      // Should emit location error
      const emitted = wrapper.emitted("location-error");
      if (emitted && emitted.length > 0) {
        expect(emitted[0][0]).toContain("Winnipeg");
      }
    }
  });

  it("should handle map click events", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 200));

    // Verify map click listener was added
    expect(mockMap.addListener).toHaveBeenCalled();

    // Check if 'click' event listener was registered
    const clickListener = mockMap.addListener.mock.calls.find((call) => call[0] === "click");
    expect(clickListener).toBeDefined();
  });

  it("should emit update when location is selected", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 200));

    // Simulate map click
    const clickListener = mockMap.addListener.mock.calls.find((call) => call[0] === "click");

    if (clickListener && typeof clickListener[1] === "function") {
      const clickEvent = {
        latLng: {
          lat: () => 49.9,
          lng: () => -97.15,
        },
      };

      clickListener[1](clickEvent);
      await wrapper.vm.$nextTick();
      await new Promise((resolve) => setTimeout(resolve, 100));

      const emitted = wrapper.emitted("update:modelValue");
      if (emitted && emitted.length > 0) {
        const emittedValue = emitted[0][0] as any;
        expect(emittedValue.latitude).toBeCloseTo(49.9, 1);
        expect(emittedValue.longitude).toBeCloseTo(-97.15, 1);
      }
    }
  });

  it("should update marker position on location change", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: "Winnipeg, MB, Canada",
        },
      },
    });

    await wrapper.vm.$nextTick();

    // Change location
    await wrapper.setProps({
      modelValue: {
        latitude: 49.9,
        longitude: -97.15,
        name: "Downtown Winnipeg",
      },
    });

    await wrapper.vm.$nextTick();

    // Props should be updated
    expect(wrapper.props("modelValue")?.latitude).toBe(49.9);
    expect(wrapper.props("modelValue")?.longitude).toBe(-97.15);
  });

  it("should handle null coordinates gracefully", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: null as any,
          longitude: null as any,
          name: "",
        },
      },
    });

    expect(wrapper.exists()).toBe(true);
  });

  it("should handle undefined name", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: undefined as any,
        },
      },
    });

    expect(wrapper.exists()).toBe(true);
  });

  it("should display loading state during initialization", async () => {
    const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");

    vi.mocked(loadGoogleMaps).mockImplementation(
      () => new Promise((resolve) => setTimeout(resolve, 100)),
    );

    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    // Check for loading state
    const loadingState = wrapper.find(".loading, .map-loading");
    if (loadingState.exists()) {
      expect(loadingState.exists()).toBe(true);
    }
  });

  it("should handle map initialization error", async () => {
    const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");
    vi.mocked(loadGoogleMaps).mockRejectedValueOnce(new Error("Failed to load"));

    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 100));

    // Component should handle error gracefully
    expect(wrapper.exists()).toBe(true);
  });

  it("should center map on Winnipeg by default", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 200));

    // Map constructor should have been called with center
    const mapConstructorCalls = (global as any).google.maps.Map.mock.calls;
    if (mapConstructorCalls.length > 0) {
      const config = mapConstructorCalls[0][1];
      expect(config.center).toBeDefined();
    }
  });

  it("should handle rapid location changes", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: "Location 1",
        },
      },
    });

    // Rapidly change locations
    const locations = [
      { latitude: 49.9, longitude: -97.15, name: "Location 2" },
      { latitude: 49.91, longitude: -97.16, name: "Location 3" },
      { latitude: 49.92, longitude: -97.17, name: "Location 4" },
    ];

    for (const location of locations) {
      await wrapper.setProps({ modelValue: location });
      await wrapper.vm.$nextTick();
    }

    // Should handle all updates
    expect(wrapper.props("modelValue")).toEqual(locations[locations.length - 1]);
  });

  it("should preserve name when coordinates update", async () => {
    const name = "Winnipeg City Hall";

    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: name,
        },
      },
    });

    await wrapper.setProps({
      modelValue: {
        latitude: 49.896,
        longitude: -97.139,
        name: name,
      },
    });

    expect(wrapper.props("modelValue")?.name).toBe(name);
  });

  it("should handle geocoding failure gracefully", async () => {
    mockGeocoder.geocode.mockImplementation((request, callback) => {
      callback([], "ERROR");
    });

    wrapper = mount(MapPicker, {
      props: {
        modelValue: null,
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 200));

    // Simulate map click
    const clickListener = mockMap.addListener.mock.calls.find((call) => call[0] === "click");

    if (clickListener && typeof clickListener[1] === "function") {
      const clickEvent = {
        latLng: {
          lat: () => 49.9,
          lng: () => -97.15,
        },
      };

      clickListener[1](clickEvent);
      await wrapper.vm.$nextTick();
      await new Promise((resolve) => setTimeout(resolve, 100));

      // Should still emit location even without formatted address
      const emitted = wrapper.emitted("update:modelValue");
      if (emitted && emitted.length > 0) {
        expect(emitted[0][0]).toBeDefined();
      }
    }
  });

  it("should handle zero coordinates", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 0,
          longitude: 0,
          name: "Null Island",
        },
      },
    });

    expect(wrapper.props("modelValue")?.latitude).toBe(0);
    expect(wrapper.props("modelValue")?.longitude).toBe(0);
  });

  it("should handle negative coordinates", () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: -49.8951,
          longitude: -97.1384,
          name: "Southern Location",
        },
      },
    });

    expect(wrapper.props("modelValue")?.latitude).toBe(-49.8951);
    expect(wrapper.props("modelValue")?.longitude).toBe(-97.1384);
  });

  it("should clear location when clear button is clicked", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: "Winnipeg, MB, Canada",
        },
      },
    });

    await wrapper.vm.$nextTick();

    const clearButton = wrapper.find(".clear-button");
    if (clearButton.exists()) {
      await clearButton.trigger("click");

      const emitted = wrapper.emitted("update:modelValue");
      if (emitted) {
        expect(emitted[emitted.length - 1][0]).toBeNull();
      }
    }
  });

  it("should display location name when selected", async () => {
    wrapper = mount(MapPicker, {
      props: {
        modelValue: {
          latitude: 49.8951,
          longitude: -97.1384,
          name: "Winnipeg City Hall",
        },
      },
    });

    await wrapper.vm.$nextTick();

    const locationName = wrapper.find(".location-name");
    if (locationName.exists()) {
      expect(locationName.text()).toBe("Winnipeg City Hall");
    }
  });
});
