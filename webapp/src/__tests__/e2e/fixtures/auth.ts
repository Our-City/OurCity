import { test as base } from "@playwright/test";

/**
 * Test fixture for authenticated user sessions
 * Extend this to add authentication state to your tests
 */
export const test = base.extend({
  // Add authenticated context here if needed
  // For example, you can create a fixture that logs in a user before each test
});

export { expect } from "@playwright/test";
