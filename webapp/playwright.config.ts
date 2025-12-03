/*
  Config file for Playwright end-to-end tests.

  Generated with the assistance of AI.
    Copilot assisted in the initial creation of this file.
*/

import { defineConfig, devices } from "@playwright/test";

/**
 * See https://playwright.dev/docs/test-configuration.
 */
export default defineConfig({
  testDir: "./src/__tests__/e2e",

  /* Run tests in files in parallel */
  fullyParallel: true,

  /* fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,

  /* retry on CI only */
  retries: process.env.CI ? 2 : 0,

  /* opt out of parallel tests on CI. */
  workers: process.env.CI ? 1 : undefined,

  /* reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [["html"], ["list"]],

  /* shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* base URL to use in actions like `await page.goto('/')`. */
    baseURL: "http://localhost:5173",

    /* collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: "on-first-retry",

    /* screenshot on failure */
    screenshot: "only-on-failure",
  },

  /* configure projects for major browsers */
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },

    {
      name: "firefox",
      use: { ...devices["Desktop Firefox"] },
    },

    {
      name: "webkit",
      use: { ...devices["Desktop Safari"] },
    },
  ],

  /* local local dev server before starting the tests */
  webServer: {
    command: "npm run dev",
    url: "http://localhost:5173",
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000,
  },
});
