/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to outline assertions for the profile bookmarks flow.
import { test, expect } from "@playwright/test";
import { createTestUser, createTestPost, login } from "./helpers";

test.describe("Profile Bookmarks", () => {
  test.beforeAll(async () => {
    await createTestUser();
  });

  test("should list bookmarked posts when bookmarks tab is selected", async ({ page }) => {
    const uniqueTitle = `Profile Bookmark ${Date.now()}`;
    const postId = await createTestPost({ title: uniqueTitle });

    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${postId}`);
    await page.waitForLoadState("networkidle");

    const dropdownButton = page
      .locator(".post-header button")
      .filter({ has: page.locator(".pi-ellipsis-v") })
      .first();
    await dropdownButton.click();
    await page
      .locator(".oc-dropdown-menu li")
      .filter({ hasText: /bookmark/i })
      .first()
      .click();

    const bookmarkToast = page
      .locator(".p-toast-message")
      .filter({ hasText: /post bookmarked/i });
    await expect(bookmarkToast).toBeVisible({ timeout: 5000 });

    await page.route("**/me", (route) => {
      route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          id: "profile-test-user",
          username: "testuser",
          postIds: [],
          commentIds: [],
          isAdmin: false,
          isBanned: false,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }),
      });
    });

    await page.goto("/profile");
    await page.waitForLoadState("networkidle");
    const bookmarksTab = page.getByRole("button", { name: /bookmarks/i });
    await bookmarksTab.waitFor({ state: "visible" });
    await bookmarksTab.click();

    const bookmarkList = page.locator('[data-testid="post-list"]');
    await expect(bookmarkList).toContainText(uniqueTitle);

    // Clean up bookmark so other tests start from a known state
    await page.goto(`/posts/${postId}`);
    await page.waitForLoadState("networkidle");

    const dropdownButtonCleanup = page
      .locator(".post-header button")
      .filter({ has: page.locator(".pi-ellipsis-v") })
      .first();
    await dropdownButtonCleanup.click();
    await page
      .locator(".oc-dropdown-menu li")
      .filter({ hasText: /remove bookmark/i })
      .first()
      .click();

    const removalToast = page
      .locator(".p-toast-message")
      .filter({ hasText: /bookmark removed/i });
    await expect(removalToast).toBeVisible({ timeout: 5000 });
  });
});
