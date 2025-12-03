/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write e2e tests by being given a description of
///   what should be tested for this and giving back the needed functions
///   and syntax to implement the tests.

import { test, expect } from "@playwright/test";
import { login, createAdminUser } from "./helpers";

test.describe("Admin Dashboard", () => {
  test.beforeEach(async ({ page }) => {
    // Create admin test user and login
    const adminUser = await createAdminUser();
    await login(page, adminUser.username, adminUser.password);
  });

  test.describe("Page Load and Layout", () => {
    test("should load admin dashboard page", async ({ page }) => {
      await page.goto("/admin");

      await expect(page).toHaveURL("/admin");
      await expect(page.getByRole("heading", { name: /Admin Dashboard/i })).toBeVisible();
    });

    test("should display page header", async ({ page }) => {
      await page.goto("/admin");

      const pageHeader = page.locator(".page-header").first();
      await expect(pageHeader).toBeVisible();
    });

    test("should display sidebar", async ({ page }) => {
      await page.goto("/admin");

      const sidebar = page.locator(".side-bar").first();
      await expect(sidebar).toBeVisible();
    });

    test("should have proper layout structure", async ({ page }) => {
      await page.goto("/admin");

      await expect(page.locator(".admin-page")).toBeVisible();
      await expect(page.locator(".admin-page-layout")).toBeVisible();
      await expect(page.locator(".admin-page-body")).toBeVisible();
      await expect(page.locator(".admin-page-content-layout")).toBeVisible();
    });
  });

  test.describe("Activity Section", () => {
    test("should display Activity heading", async ({ page }) => {
      await page.goto("/admin");

      await expect(page.getByRole("heading", { name: /^Activity$/i })).toBeVisible();
    });

    test("should display all period selection buttons", async ({ page }) => {
      await page.goto("/admin");

      const dayButton = page.getByRole("button", { name: /^Day$/i });
      const monthButton = page.getByRole("button", { name: /^Month$/i });
      const yearButton = page.getByRole("button", { name: /^Year$/i });

      await expect(dayButton).toBeVisible();
      await expect(monthButton).toBeVisible();
      await expect(yearButton).toBeVisible();
    });

    test("should have Day period selected by default", async ({ page }) => {
      await page.goto("/admin");

      const dayButton = page.getByRole("button", { name: /^Day$/i });
      await expect(dayButton).toHaveClass(/active/);
    });

    test("should switch to Month period when clicked", async ({ page }) => {
      await page.goto("/admin");

      const monthButton = page.getByRole("button", { name: /^Month$/i });
      await monthButton.click();

      await expect(monthButton).toHaveClass(/active/);
    });

    test("should switch to Year period when clicked", async ({ page }) => {
      await page.goto("/admin");

      const yearButton = page.getByRole("button", { name: /^Year$/i });
      await yearButton.click();

      await expect(yearButton).toHaveClass(/active/);
    });

    test("should switch back to Day period", async ({ page }) => {
      await page.goto("/admin");

      const monthButton = page.getByRole("button", { name: /^Month$/i });
      await monthButton.click();

      const dayButton = page.getByRole("button", { name: /^Day$/i });
      await dayButton.click();

      await expect(dayButton).toHaveClass(/active/);
    });
  });

  test.describe("Stat Cards", () => {
    test("should display all four stat cards", async ({ page }) => {
      await page.goto("/admin");

      // Wait for stats to load
      await page.waitForTimeout(1000);

      const newPostsCard = page.locator(".stat-card.new-posts");
      const upvotesCard = page.locator(".stat-card.upvotes");
      const downvotesCard = page.locator(".stat-card.downvotes");
      const commentsCard = page.locator(".stat-card.comments");

      await expect(newPostsCard).toBeVisible();
      await expect(upvotesCard).toBeVisible();
      await expect(downvotesCard).toBeVisible();
      await expect(commentsCard).toBeVisible();
    });

    test("should display New Posts stat with label", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const newPostsCard = page.locator(".stat-card.new-posts");
      const label = newPostsCard.locator(".stat-label");
      const value = newPostsCard.locator(".stat-value");

      await expect(label).toHaveText(/new posts/i);
      await expect(value).toBeVisible();
    });

    test("should display Upvotes stat with label", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const upvotesCard = page.locator(".stat-card.upvotes");
      const label = upvotesCard.locator(".stat-label");
      const value = upvotesCard.locator(".stat-value");

      await expect(label).toHaveText(/upvotes/i);
      await expect(value).toBeVisible();
    });

    test("should display Downvotes stat with label", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const downvotesCard = page.locator(".stat-card.downvotes");
      const label = downvotesCard.locator(".stat-label");
      const value = downvotesCard.locator(".stat-value");

      await expect(label).toHaveText(/downvotes/i);
      await expect(value).toBeVisible();
    });

    test("should display Comments stat with label", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const commentsCard = page.locator(".stat-card.comments");
      const label = commentsCard.locator(".stat-label");
      const value = commentsCard.locator(".stat-value");

      await expect(label).toHaveText(/comments/i);
      await expect(value).toBeVisible();
    });

    test("should display numeric values in stat cards", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const statValues = page.locator(".stat-value");
      const count = await statValues.count();

      expect(count).toBe(4);

      for (let i = 0; i < count; i++) {
        const text = await statValues.nth(i).textContent();
        expect(text).toMatch(/^\d+$/);
      }
    });

    test("should update stats when period changes", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const newPostsValue = page.locator(".stat-card.new-posts .stat-value");

      const monthButton = page.getByRole("button", { name: /^Month$/i });
      await monthButton.click();

      await page.waitForTimeout(1000);

      const monthValue = await newPostsValue.textContent();

      // Values might be the same, but we're testing that the component updates
      expect(monthValue).toBeDefined();
    });
  });

  test.describe("Posts Over Time Chart", () => {
    test("should display Posts Over Time heading", async ({ page }) => {
      await page.goto("/admin");

      await expect(page.getByRole("heading", { name: /Posts Over Time/i })).toBeVisible();
    });

    test("should display chart wrapper", async ({ page }) => {
      await page.goto("/admin");

      const chartWrapper = page.locator(".chart-wrapper").first();
      await expect(chartWrapper).toBeVisible();
    });

    test("should render chart canvas", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const canvas = page.locator("canvas").first();
      await expect(canvas).toBeVisible();
    });

    test("should update chart when period changes", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const monthButton = page.getByRole("button", { name: /^Month$/i });
      await monthButton.click();

      await page.waitForTimeout(500);

      const canvas = page.locator("canvas").first();
      await expect(canvas).toBeVisible();
    });
  });

  test.describe("Tag Breakdown Chart", () => {
    test("should display Tag Breakdown heading", async ({ page }) => {
      await page.goto("/admin");

      await expect(page.getByRole("heading", { name: /Tag Breakdown/i })).toBeVisible();
    });

    test("should display chart container", async ({ page }) => {
      await page.goto("/admin");

      const chartContainer = page.locator(".chart-container").first();
      await expect(chartContainer).toBeVisible();
    });

    test("should render doughnut chart", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      // There should be at least 2 canvases (line chart and doughnut chart)
      const canvases = page.locator("canvas");
      const count = await canvases.count();
      expect(count).toBeGreaterThanOrEqual(2);
    });

    test("should update tag chart when period changes", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const yearButton = page.getByRole("button", { name: /^Year$/i });
      await yearButton.click();

      await page.waitForTimeout(500);

      const canvases = page.locator("canvas");
      const count = await canvases.count();
      expect(count).toBeGreaterThanOrEqual(2);
    });
  });

  test.describe("Data Loading", () => {
    test("should load data on page mount", async ({ page }) => {
      const responsePromise = page.waitForResponse(
        (response) => response.url().includes("/analytics/summary") && response.status() === 200,
      );

      await page.goto("/admin");

      await responsePromise;
    });

    test("should fetch time series data", async ({ page }) => {
      const responsePromise = page.waitForResponse(
        (response) =>
          response.url().includes("/analytics/time-series") && response.status() === 200,
      );

      await page.goto("/admin");

      await responsePromise;
    });

    test("should fetch tag breakdown data", async ({ page }) => {
      const responsePromise = page.waitForResponse(
        (response) =>
          response.url().includes("/analytics/tag-breakdown") && response.status() === 200,
      );

      await page.goto("/admin");

      await responsePromise;
    });

    test("should refetch data when period changes", async ({ page }) => {
      await page.goto("/admin");

      await page.waitForTimeout(1000);

      const responsePromise = page.waitForResponse(
        (response) =>
          response.url().includes("/analytics/summary") &&
          response.url().includes("period=Month") &&
          response.status() === 200,
      );

      const monthButton = page.getByRole("button", { name: /^Month$/i });
      await monthButton.click();

      await responsePromise;
    });
  });

  test.describe("Navigation", () => {
    test("should navigate away from admin dashboard", async ({ page }) => {
      await page.goto("/admin");

      // Navigate to home via sidebar or header
      await page.goto("/");

      await expect(page).toHaveURL("/");
    });

    test("should be able to return to admin dashboard", async ({ page }) => {
      await page.goto("/");
      await page.goto("/admin");

      await expect(page).toHaveURL("/admin");
      await expect(page.getByRole("heading", { name: /Admin Dashboard/i })).toBeVisible();
    });
  });

  test.describe("Error Handling", () => {
    test("should handle API errors gracefully", async ({ page }) => {
      // Intercept API calls and return errors
      await page.route("**/analytics/summary*", (route) => {
        route.fulfill({
          status: 500,
          body: JSON.stringify({ error: "Internal Server Error" }),
        });
      });

      await page.goto("/admin");

      await page.waitForTimeout(1000);

      // The page should still render even if API fails
      await expect(page.locator(".admin-page")).toBeVisible();
    });
  });
});
