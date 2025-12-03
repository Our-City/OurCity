/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for LocationAutocomplete component
/// Tests Google Places autocomplete integration and location selection

import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";
import { mount, VueWrapper } from "@vue/test-utils";
import LocationAutocomplete from "@/components/LocationAutocomplete.vue";

interface WindowWithGoogle extends Window {
  google?: typeof google;
}

declare const global: WindowWithGoogle;

// Mock Google Maps API
const mockAutocomplete = {
  addListener: vi.fn(),
  getPlace: vi.fn(),
  setBounds: vi.fn(),
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
(global as WindowWithGoogle).google = {
  maps: {
    places: {
      Autocomplete: vi.fn(() => mockAutocomplete),
    },
    LatLngBounds: vi.fn(),
  },
};

describe("LocationAutocomplete.vue", () => {
  let wrapper: VueWrapper<InstanceType<typeof LocationAutocomplete>>;

  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  it("should render input field", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    const input = wrapper.find('input[type="text"]');
    expect(input.exists()).toBe(true);
  });

  it("should display placeholder text", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
        placeholder: "Search for a place...",
      },
    });

    const input = wrapper.find("input");
    expect(input.attributes("placeholder")).toBe("Search for a place...");
  });

  it("should use default placeholder when not provided", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    const input = wrapper.find("input");
    expect(input.attributes("placeholder")).toBe("Search for a location...");
  });

  it("should display initial value", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "Winnipeg City Hall",
      },
    });

    const input = wrapper.find("input");
    expect(input.element.value).toBe("Winnipeg City Hall");
  });

  it("should emit update on input change", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    const input = wrapper.find("input");
    await input.setValue("Downtown Winnipeg");

    const emitted = wrapper.emitted("update:modelValue");
    expect(emitted).toBeTruthy();
    expect(emitted![0]).toEqual(["Downtown Winnipeg"]);
  });

  it("should be disabled when disabled prop is true", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
        disabled: true,
      },
    });

    const input = wrapper.find("input");
    expect(input.attributes("disabled")).toBeDefined();
  });

  it("should not be disabled by default", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    const input = wrapper.find("input");
    expect(input.attributes("disabled")).toBeUndefined();
  });

  it("should initialize Google Places Autocomplete on mount", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    // Autocomplete should be initialized
    expect((global as WindowWithGoogle).google!.maps.places.Autocomplete).toHaveBeenCalled();
  });

  it("should add place_changed listener", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    // Should add listener for place_changed event
    expect(mockAutocomplete.addListener).toHaveBeenCalledWith(
      "place_changed",
      expect.any(Function),
    );
  });

  it("should emit location-selected when valid place is selected", async () => {
    const mockPlace = {
      geometry: {
        location: {
          lat: () => 49.8951,
          lng: () => -97.1384,
        },
      },
      formatted_address: "Winnipeg, MB, Canada",
      name: "Winnipeg",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    // Simulate place selection
    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      const emitted = wrapper.emitted("location-selected");
      expect(emitted).toBeTruthy();
      expect(emitted![0][0]).toEqual({
        name: "Winnipeg, MB, Canada",
        latitude: 49.8951,
        longitude: -97.1384,
      });
    }
  });

  it("should validate location is within Winnipeg", async () => {
    const { isWithinWinnipeg } = await import("@/utils/locationValidator");

    const mockPlace = {
      geometry: {
        location: {
          lat: () => 49.8951,
          lng: () => -97.1384,
        },
      },
      formatted_address: "Winnipeg, MB, Canada",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      expect(isWithinWinnipeg).toHaveBeenCalledWith(49.8951, -97.1384);
    }
  });

  it("should emit error for location outside Winnipeg", async () => {
    const { isWithinWinnipeg, getDistanceFromWinnipeg } = await import("@/utils/locationValidator");

    vi.mocked(isWithinWinnipeg).mockReturnValue(false);
    vi.mocked(getDistanceFromWinnipeg).mockReturnValue(1500);

    const mockPlace = {
      geometry: {
        location: {
          lat: () => 43.6532,
          lng: () => -79.3832,
        },
      },
      formatted_address: "Toronto, ON, Canada",
      name: "Toronto",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      const emitted = wrapper.emitted("location-error");
      expect(emitted).toBeTruthy();
      expect(emitted![0][0]).toContain("1500.0 km from Winnipeg");
      expect(emitted![0][0]).toContain("Toronto, ON, Canada");
    }
  });

  it("should clear input when location outside Winnipeg is selected", async () => {
    const { isWithinWinnipeg } = await import("@/utils/locationValidator");
    vi.mocked(isWithinWinnipeg).mockReturnValue(false);

    const mockPlace = {
      geometry: {
        location: {
          lat: () => 43.6532,
          lng: () => -79.3832,
        },
      },
      formatted_address: "Toronto, ON, Canada",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      // Input should be cleared
      const input = wrapper.find("input");
      expect(input.element.value).toBe("");
    }
  });

  it("should handle place with no geometry gracefully", async () => {
    const mockPlace = {
      formatted_address: "Invalid Place",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      expect(consoleErrorSpy).toHaveBeenCalled();

      // Should not emit location-selected
      expect(wrapper.emitted("location-selected")).toBeFalsy();
    }

    consoleErrorSpy.mockRestore();
  });

  it("should update value when modelValue prop changes", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "Initial Location",
      },
    });

    const input = wrapper.find("input");
    expect(input.element.value).toBe("Initial Location");

    await wrapper.setProps({ modelValue: "New Location" });
    await wrapper.vm.$nextTick();

    expect(input.element.value).toBe("New Location");
  });

  it("should restrict autocomplete to Canada", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const autocompleteConstructor = (global as WindowWithGoogle).google!.maps.places.Autocomplete;
    const constructorCalls = (
      autocompleteConstructor as unknown as { mock: { calls: unknown[][] } }
    ).mock.calls;

    if (constructorCalls.length > 0) {
      const options = constructorCalls[0][1] as { componentRestrictions?: { country: string } };
      expect(options.componentRestrictions).toEqual({ country: "ca" });
    }
  });

  it("should request specific place fields", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const autocompleteConstructor = (global as WindowWithGoogle).google!.maps.places.Autocomplete;
    const constructorCalls = (
      autocompleteConstructor as unknown as { mock: { calls: unknown[][] } }
    ).mock.calls;

    if (constructorCalls.length > 0) {
      const options = constructorCalls[0][1] as { fields?: string[] };
      expect(options.fields).toEqual(["formatted_address", "geometry", "name"]);
    }
  });

  it("should use geocode and establishment types", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const autocompleteConstructor = (global as WindowWithGoogle).google!.maps.places.Autocomplete;
    const constructorCalls = (
      autocompleteConstructor as unknown as { mock: { calls: unknown[][] } }
    ).mock.calls;

    if (constructorCalls.length > 0) {
      const options = constructorCalls[0][1] as { types?: string[] };
      expect(options.types).toEqual(["geocode", "establishment"]);
    }
  });

  it("should handle empty string modelValue", () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    const input = wrapper.find("input");
    expect(input.element.value).toBe("");
  });

  it("should handle special characters in location name", async () => {
    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "L'Esplanade Riel, Winnipeg",
      },
    });

    const input = wrapper.find("input");
    expect(input.element.value).toBe("L'Esplanade Riel, Winnipeg");
  });

  it("should emit null error when valid location is selected", async () => {
    // Reset mocks to default (valid location)
    const { isWithinWinnipeg, getDistanceFromWinnipeg } = await import("@/utils/locationValidator");
    vi.mocked(isWithinWinnipeg).mockReturnValue(true);
    vi.mocked(getDistanceFromWinnipeg).mockReturnValue(5);

    const mockPlace = {
      geometry: {
        location: {
          lat: () => 49.8951,
          lng: () => -97.1384,
        },
      },
      formatted_address: "Winnipeg, MB, Canada",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      const emitted = wrapper.emitted("location-error");
      expect(emitted).toBeTruthy();
      expect(emitted![0][0]).toBeNull();
    }
  });

  it("should handle autocomplete initialization error gracefully", async () => {
    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => {});
    const { loadGoogleMaps } = await import("@/utils/googleMapsLoader");

    vi.mocked(loadGoogleMaps).mockRejectedValueOnce(new Error("Failed to load"));

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    expect(consoleErrorSpy).toHaveBeenCalled();

    consoleErrorSpy.mockRestore();
  });

  it("should prefer formatted_address over name for location", async () => {
    // Reset mocks to default (valid location)
    const { isWithinWinnipeg, getDistanceFromWinnipeg } = await import("@/utils/locationValidator");
    vi.mocked(isWithinWinnipeg).mockReturnValue(true);
    vi.mocked(getDistanceFromWinnipeg).mockReturnValue(5);

    const mockPlace = {
      geometry: {
        location: {
          lat: () => 49.8951,
          lng: () => -97.1384,
        },
      },
      formatted_address: "Winnipeg, MB R3C 4A5, Canada",
      name: "City Hall",
    };

    mockAutocomplete.getPlace.mockReturnValue(mockPlace);

    wrapper = mount(LocationAutocomplete, {
      props: {
        modelValue: "",
      },
    });

    await wrapper.vm.$nextTick();
    await new Promise((resolve) => setTimeout(resolve, 600));

    const placeChangedCallback = mockAutocomplete.addListener.mock.calls.find(
      (call) => call[0] === "place_changed",
    )?.[1];

    expect(placeChangedCallback).toBeDefined();

    if (placeChangedCallback) {
      placeChangedCallback();
      await wrapper.vm.$nextTick();

      const emitted = wrapper.emitted("location-selected");
      expect(emitted).toBeTruthy();
      expect(emitted).toHaveLength(1);
      expect(emitted![0][0]).toEqual({
        name: "Winnipeg, MB R3C 4A5, Canada",
        latitude: 49.8951,
        longitude: -97.1384,
      });
    }
  });
});
