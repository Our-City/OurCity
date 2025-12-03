/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for Google Maps API loader
/// Tests library loading, error handling, and state management

import { describe, it, expect, beforeEach, afterEach, vi } from "vitest";

// Type for window with google property
interface WindowWithGoogle extends Window {
  google?: typeof google;
}

declare const window: WindowWithGoogle;

// Mock the Google Maps API
const mockGoogleMaps = {
  maps: {
    Map: vi.fn(),
    Marker: vi.fn(),
    InfoWindow: vi.fn(),
    places: {
      Autocomplete: vi.fn(),
      PlacesService: vi.fn(),
    },
    geocoding: {
      Geocoder: vi.fn(),
    },
    marker: {
      AdvancedMarkerElement: vi.fn(),
    },
    visualization: {
      HeatmapLayer: vi.fn(),
    },
    Circle: vi.fn(),
    LatLng: vi.fn(),
    LatLngBounds: vi.fn(),
  },
};

describe("googleMapsLoader", () => {
  let loadGoogleMaps: () => Promise<void>;
  let isGoogleMapsLoaded: () => boolean;

  beforeEach(async () => {
    // Clear any existing Google Maps instance
    delete window.google;

    // Clear document scripts
    const scripts = document.querySelectorAll('script[src*="maps.googleapis.com"]');
    scripts.forEach((script) => script.remove());

    // Force module reimport to reset state
    vi.resetModules();
    const module = await import("@/utils/googleMapsLoader");
    loadGoogleMaps = module.loadGoogleMaps;
    isGoogleMapsLoaded = module.isGoogleMapsLoaded;
  });

  afterEach(() => {
    vi.clearAllMocks();
    delete window.google;
    const scripts = document.querySelectorAll('script[src*="maps.googleapis.com"]');
    scripts.forEach((script) => script.remove());
  });

  describe("loadGoogleMaps", () => {
    it("should load Google Maps script", async () => {
      const loadPromise = loadGoogleMaps();

      // Wait for script to be added
      await new Promise((resolve) => setTimeout(resolve, 10));

      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      expect(script).toBeTruthy();
      expect(script?.getAttribute("src")).toContain(
        "libraries=places,geocoding,marker,visualization",
      );

      // Simulate successful load
      window.google = mockGoogleMaps as unknown as typeof google;
      script?.dispatchEvent(new Event("load"));

      await expect(loadPromise).resolves.toBeUndefined();
    });

    it("should return existing promise if already loading", async () => {
      const promise1 = loadGoogleMaps();
      const promise2 = loadGoogleMaps();

      expect(promise1).toBe(promise2);

      // Wait and simulate load
      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      window.google = mockGoogleMaps as unknown as typeof google;
      script?.dispatchEvent(new Event("load"));

      await promise1;
    });

    it("should resolve immediately if already loaded", async () => {
      // Pre-load Google Maps
      window.google = mockGoogleMaps as unknown as typeof google;

      const startTime = Date.now();
      await loadGoogleMaps();
      const endTime = Date.now();

      // Should resolve almost immediately (within 100ms)
      expect(endTime - startTime).toBeLessThan(100);
    });

    it("should reject on script load error", async () => {
      const loadPromise = loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      script?.dispatchEvent(new Event("error"));

      await expect(loadPromise).rejects.toThrow("Failed to load Google Maps script");
    });

    it("should include API key in script URL", async () => {
      loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      const src = script?.getAttribute("src") || "";

      // Should contain key parameter
      expect(src).toContain("key=");
    });

    it("should load all required libraries", async () => {
      loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      const src = script?.getAttribute("src") || "";

      expect(src).toContain("libraries=places,geocoding,marker,visualization");
    });

    it("should set async attribute on script", async () => {
      loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector(
        'script[src*="maps.googleapis.com"]',
      ) as HTMLScriptElement;
      expect(script?.async).toBe(true);
    });

    it("should append script to document head", async () => {
      loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      expect(script?.parentElement?.tagName).toBe("HEAD");
    });

    it("should only create one script element", async () => {
      loadGoogleMaps();
      loadGoogleMaps();
      loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const scripts = document.querySelectorAll('script[src*="maps.googleapis.com"]');
      expect(scripts.length).toBe(1);
    });

    it("should handle multiple rapid calls correctly", async () => {
      const promises = [loadGoogleMaps(), loadGoogleMaps(), loadGoogleMaps()];

      // All should return the same promise
      expect(promises[0]).toBe(promises[1]);
      expect(promises[1]).toBe(promises[2]);

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      window.google = mockGoogleMaps as unknown as typeof google;
      script?.dispatchEvent(new Event("load"));

      await Promise.all(promises);
    });
  });

  describe("isGoogleMapsLoaded", () => {
    it("should return false when not loaded", () => {
      expect(isGoogleMapsLoaded()).toBe(false);
    });

    it("should return true when fully loaded", async () => {
      window.google = mockGoogleMaps as unknown as typeof google;

      // Trigger load
      await loadGoogleMaps();

      expect(isGoogleMapsLoaded()).toBe(true);
    });

    it("should return false if google object missing", () => {
      delete window.google;

      expect(isGoogleMapsLoaded()).toBe(false);
    });

    it("should return false if maps property missing", () => {
      window.google = {} as typeof google;

      expect(isGoogleMapsLoaded()).toBe(false);
    });

    it("should return false if places library not loaded", () => {
      window.google = {
        maps: {
          Map: vi.fn(),
          marker: {
            AdvancedMarkerElement: vi.fn(),
          },
          visualization: {
            HeatmapLayer: vi.fn(),
          },
        },
      } as unknown as typeof google;

      expect(isGoogleMapsLoaded()).toBe(false);
    });

    it("should return false if visualization library not loaded", () => {
      window.google = {
        maps: {
          Map: vi.fn(),
          places: {
            Autocomplete: vi.fn(),
          },
          marker: {
            AdvancedMarkerElement: vi.fn(),
          },
        },
      } as unknown as typeof google;

      expect(isGoogleMapsLoaded()).toBe(false);
    });

    it("should return false if marker library not loaded", () => {
      window.google = {
        maps: {
          Map: vi.fn(),
          places: {
            Autocomplete: vi.fn(),
          },
          visualization: {
            HeatmapLayer: vi.fn(),
          },
        },
      } as unknown as typeof google;

      expect(isGoogleMapsLoaded()).toBe(false);
    });

    it("should verify all required libraries are present", async () => {
      window.google = mockGoogleMaps as unknown as typeof google;

      // Trigger a load to set the internal flag
      const loadPromise = loadGoogleMaps();
      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      script?.dispatchEvent(new Event("load"));

      await loadPromise;

      expect(isGoogleMapsLoaded()).toBe(true);
      expect(window.google?.maps?.places).toBeDefined();
      expect(window.google?.maps?.marker).toBeDefined();
      expect(window.google?.maps?.visualization).toBeDefined();
    });
  });

  describe("error handling", () => {
    it("should clean up on error", async () => {
      const loadPromise = loadGoogleMaps();

      await new Promise((resolve) => setTimeout(resolve, 10));
      const script = document.querySelector('script[src*="maps.googleapis.com"]');
      script?.dispatchEvent(new Event("error"));

      await expect(loadPromise).rejects.toThrow();
    });

    it("should allow retry after error", async () => {
      // First attempt - fail
      const loadPromise1 = loadGoogleMaps();
      await new Promise((resolve) => setTimeout(resolve, 10));
      const script1 = document.querySelector('script[src*="maps.googleapis.com"]');
      script1?.dispatchEvent(new Event("error"));

      try {
        await loadPromise1;
      } catch {
        // Expected error
      }

      // Clear the failed script
      script1?.remove();

      // Reimport module to reset state
      vi.resetModules();
      const module = await import("@/utils/googleMapsLoader");
      const loadGoogleMaps2 = module.loadGoogleMaps;

      // Second attempt - succeed
      const loadPromise2 = loadGoogleMaps2();
      await new Promise((resolve) => setTimeout(resolve, 10));
      const script2 = document.querySelector('script[src*="maps.googleapis.com"]');

      window.google = mockGoogleMaps as unknown as typeof google;
      script2?.dispatchEvent(new Event("load"));

      await expect(loadPromise2).resolves.toBeUndefined();
    });
  });
});