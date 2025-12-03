/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help scaffold Playwright tests for the post reporting flow in a similar manner to the other e2e tests.

import { test, expect, request } from "@playwright/test";
import { createTestUser, createTestPost, login } from "./helpers";

const REPORTER_PASSWORD = "ReporterPass123!"; // mock password for reporter user
let reporterUsername: string;
let testPostId: string;

async function createUser(username: string, password: string) {
  const api = await request.newContext();
  const response = await api.post("http://localhost:8000/apis/v1/users", {
    data: {
      username,
      password,
      passwordConfirm: password,
    },
  });

  if (!response.ok() && response.status() !== 409) {
    throw new Error(`Failed to create user: ${response.status()} ${await response.text()}`);
  }

  await api.dispose();
}

test.describe("Post Reporting", () => {
  test.beforeAll(async () => {
    await createTestUser();
    reporterUsername = `reporter_${Date.now()}`;
    await createUser(reporterUsername, REPORTER_PASSWORD);
    testPostId = await createTestPost({ title: `Report Flow Test ${Date.now()}` });
  });

  test.beforeEach(async ({ page }) => {
    await page.context().clearCookies();
  });

  test("hides report option for the post author", async ({ page }) => {
    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${testPostId}`);
    await page.waitForLoadState("networkidle");

    const dropdownButton = page
      .locator(".post-header button")
      .filter({ has: page.locator(".pi-ellipsis-v") })
      .first();
    await dropdownButton.click();

    const reportOption = page
      .locator(".oc-dropdown-menu li, ul li")
      .filter({ hasText: /^\s*report\b/i });
    await expect(reportOption).toHaveCount(0);
  });
});
