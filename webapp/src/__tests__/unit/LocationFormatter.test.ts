/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write unit tests for the components by being given
///   a description of what exactly should be tested for this component and giving
///   back the needed functions and syntax to implement the tests.

/// Unit tests for location formatting utilities
/// Tests postal code removal from various Canadian address formats

import { describe, it, expect } from "vitest";
import { removePostalCode } from "@/utils/locationFormatter";

describe("locationFormatter", () => {
  describe("removePostalCode", () => {
    it("should remove postal code from full address", () => {
      const input = "575 Berry St, Winnipeg, MB R3H 0S2, Canada";
      const expected = "575 Berry St, Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should remove postal code from city address", () => {
      const input = "Winnipeg, MB R3T, Canada";
      const expected = "Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should remove partial postal code (3 characters)", () => {
      const input = "Downtown, Winnipeg, MB R3B, Canada";
      const expected = "Downtown, Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should handle plus codes without modification", () => {
      const input = "QPX2+4V Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(input);
    });

    it("should handle postal codes without spaces", () => {
      const input = "Winnipeg, MB R3H0S2, Canada";
      const expected = "Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should handle lowercase postal codes", () => {
      const input = "Winnipeg, MB r3h 0s2, Canada";
      const expected = "Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should return empty string for null input", () => {
      expect(removePostalCode(null as unknown as string)).toBe("");
    });

    it("should return empty string for undefined input", () => {
      expect(removePostalCode(undefined)).toBe("");
    });

    it("should return empty string for empty string", () => {
      expect(removePostalCode("")).toBe("");
    });

    it("should handle location without postal code", () => {
      const input = "Downtown Winnipeg, Manitoba, Canada";
      expect(removePostalCode(input)).toBe(input);
    });

    it("should clean up double commas after removal", () => {
      const input = "Winnipeg, R3H 0S2, Canada";
      const expected = "Winnipeg, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should remove trailing comma if present", () => {
      const input = "Winnipeg, MB R3H 0S2,";
      const expected = "Winnipeg, MB";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should normalize multiple spaces", () => {
      const input = "Winnipeg,  MB   R3H 0S2,  Canada";
      const expected = "Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should handle multiple postal codes in one string", () => {
      const input = "From R3H 0S2 to R3T 1A1, Winnipeg";
      const expected = "From to, Winnipeg";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should preserve street addresses", () => {
      const input = "123 Main Street, Winnipeg, MB R3C 1A1, Canada";
      const expected = "123 Main Street, Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });

    it("should handle addresses with suite numbers", () => {
      const input = "Suite 400, 575 Berry St, Winnipeg, MB R3H 0S2, Canada";
      const expected = "Suite 400, 575 Berry St, Winnipeg, MB, Canada";
      expect(removePostalCode(input)).toBe(expected);
    });
  });
});
