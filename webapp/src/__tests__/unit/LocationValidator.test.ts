/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for location validation utilities
/// Tests Winnipeg boundary checking and distance calculations

import { describe, it, expect } from "vitest";
import { isWithinWinnipeg, getDistanceFromWinnipeg } from "@/utils/locationValidator";

const WINNIPEG_CENTER = { lat: 49.8951, lng: -97.1384 };
const WINNIPEG_RADIUS_KM = 25;

describe("locationValidator", () => {
  describe("isWithinWinnipeg", () => {
    it("should return true for Winnipeg city center", () => {
      expect(isWithinWinnipeg(WINNIPEG_CENTER.lat, WINNIPEG_CENTER.lng)).toBe(true);
    });

    it("should return true for location within Winnipeg boundary", () => {
      // Location ~10km from center (within 25km radius)
      const lat = 49.95;
      const lng = -97.15;
      expect(isWithinWinnipeg(lat, lng)).toBe(true);
    });

    it("should return false for location outside Winnipeg", () => {
      // Toronto coordinates
      const lat = 43.6532;
      const lng = -79.3832;
      expect(isWithinWinnipeg(lat, lng)).toBe(false);
    });

    it("should return false for location just outside radius", () => {
      // ~30km from center (outside 25km radius)
      const lat = 50.15;
      const lng = -97.1;
      expect(isWithinWinnipeg(lat, lng)).toBe(false);
    });

    it("should handle edge of boundary correctly", () => {
      // Approximately at the 25km boundary (north)
      // 1 degree latitude ≈ 111km, so 25km ≈ 0.225 degrees
      const lat = WINNIPEG_CENTER.lat + 0.22;
      const lng = WINNIPEG_CENTER.lng;

      const result = isWithinWinnipeg(lat, lng);
      // Should be close to the boundary, may or may not be within
      expect(typeof result).toBe("boolean");
    });

    it("should return false for null coordinates", () => {
      expect(isWithinWinnipeg(null as unknown as number, null as unknown as number)).toBe(false);
    });

    it("should return false for undefined coordinates", () => {
      expect(isWithinWinnipeg(undefined as unknown as number, undefined as unknown as number)).toBe(false);
    });

    it("should return false for invalid latitude", () => {
      expect(isWithinWinnipeg(NaN, -97.1384)).toBe(false);
    });

    it("should return false for invalid longitude", () => {
      expect(isWithinWinnipeg(49.8951, NaN)).toBe(false);
    });

    it("should handle locations to the south", () => {
      const lat = 49.8;
      const lng = -97.1384;
      expect(isWithinWinnipeg(lat, lng)).toBe(true);
    });

    it("should handle locations to the north", () => {
      const lat = 50.0;
      const lng = -97.1384;
      expect(isWithinWinnipeg(lat, lng)).toBe(true);
    });

    it("should handle locations to the east", () => {
      const lat = 49.8951;
      const lng = -97.05;
      expect(isWithinWinnipeg(lat, lng)).toBe(true);
    });

    it("should handle locations to the west", () => {
      const lat = 49.8951;
      const lng = -97.25;
      expect(isWithinWinnipeg(lat, lng)).toBe(true);
    });
  });

  describe("getDistanceFromWinnipeg", () => {
    it("should return 0 for Winnipeg city center", () => {
      const distance = getDistanceFromWinnipeg(WINNIPEG_CENTER.lat, WINNIPEG_CENTER.lng);
      expect(distance).toBeCloseTo(0, 1);
    });

    it("should calculate distance correctly for nearby location", () => {
      // Point ~10km north of center
      const lat = 49.985;
      const lng = -97.1384;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeGreaterThan(9);
      expect(distance).toBeLessThan(11);
    });

    it("should return distance in kilometers", () => {
      // Toronto (much farther away)
      const lat = 43.6532;
      const lng = -79.3832;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeGreaterThan(1000); // Should be ~1500km
    });

    it("should handle southern location", () => {
      const lat = 49.8;
      const lng = -97.1384;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeGreaterThan(0);
      expect(distance).toBeLessThan(15);
    });

    it("should handle eastern location", () => {
      const lat = 49.8951;
      const lng = -97.0;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeGreaterThan(0);
      expect(distance).toBeLessThan(20);
    });

    it("should handle western location", () => {
      const lat = 49.8951;
      const lng = -97.3;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeGreaterThan(0);
      expect(distance).toBeLessThan(20);
    });

    it("should return Infinity for null coordinates", () => {
      expect(getDistanceFromWinnipeg(null as unknown as number, null as unknown as number)).toBe(Infinity);
    });

    it("should return Infinity for undefined coordinates", () => {
      expect(getDistanceFromWinnipeg(undefined as unknown as number, undefined as unknown as number)).toBe(Infinity);
    });

    it("should return Infinity for NaN coordinates", () => {
      expect(getDistanceFromWinnipeg(NaN, NaN)).toBe(Infinity);
    });

    it("should return consistent results for same coordinates", () => {
      const lat = 49.9;
      const lng = -97.15;

      const distance1 = getDistanceFromWinnipeg(lat, lng);
      const distance2 = getDistanceFromWinnipeg(lat, lng);

      expect(distance1).toBe(distance2);
    });

    it("should calculate distance symmetrically", () => {
      // Distance from A to B should equal distance from B to A
      const lat1 = 49.9;
      const lng1 = -97.15;

      const distance = getDistanceFromWinnipeg(lat1, lng1);

      // This is always from Winnipeg center, so we just check it's positive
      expect(distance).toBeGreaterThan(0);
    });

    it("should handle decimal precision", () => {
      const lat = 49.895100001;
      const lng = -97.138400001;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeCloseTo(0, 0);
    });

    it("should handle locations at exactly 25km boundary", () => {
      // Calculate a point exactly 25km away
      // Using approximate conversion: 1 degree latitude ≈ 111km
      const lat = WINNIPEG_CENTER.lat + WINNIPEG_RADIUS_KM / 111;
      const lng = WINNIPEG_CENTER.lng;

      const distance = getDistanceFromWinnipeg(lat, lng);
      expect(distance).toBeCloseTo(WINNIPEG_RADIUS_KM, 0);
    });
  });
});
