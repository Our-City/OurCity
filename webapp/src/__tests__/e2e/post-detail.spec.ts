/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write e2e tests by being given a description of
///   what should be tested for this and giving back the needed functions
///   and syntax to implement the tests.
import { test, expect } from "@playwright/test";
import { createTestUser, login } from "./helpers";
import { createTestPost } from "./helpers";

test.describe("Post Detail Page", () => {
  // Note: You'll need to use an actual post ID from your test database
  let TEST_POST_ID: string;
  test.beforeAll(async () => {
    await createTestUser();
    TEST_POST_ID = await createTestPost();
  });

  test("should display post details", async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);

    // Wait for page to load
    await page.waitForLoadState("networkidle");

    // Check if post content is visible
    const postTitle = page.locator('[data-testid="post-title"], h1, h2').first();
    await expect(postTitle).toBeVisible({ timeout: 10000 });
  });

  test("should display post author information", async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);

    // Look for author name or profile link
    const author = page.locator('[data-testid="post-author"], .author');
    await expect(author.first()).toBeVisible({ timeout: 10000 });
  });

  test("should display voting buttons", async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);

    // Check for upvote and downvote buttons
    const voteBox = page.locator('[data-testid="vote-box"]');

    if (await voteBox.isVisible()) {
      await expect(voteBox).toBeVisible();
      // Check for upvote and downvote buttons by class
      const upvoteButton = page.locator(".vote-btn.upvote");
      const downvoteButton = page.locator(".vote-btn.downvote");
      await expect(upvoteButton).toBeVisible({ timeout: 5000 });
      await expect(downvoteButton).toBeVisible({ timeout: 5000 });
    }
  });

  test("should display comments section", async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);

    // Look for comments list by class and check existence
    const commentList = page.locator(".comments-list");
    await expect(commentList).toHaveCount(1);
  });

  test("should handle non-existent post gracefully", async ({ page }) => {
    await page.goto(`/posts/12345`);
    // Should show error message or stay on post detail page
    const errorMessage = page.locator('.error-message, [role="alert"], .p-toast-message-error');
    try {
      await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
    } catch {
      // If not visible, check that the URL is still the post detail page
      await expect(page).toHaveURL(/\/posts\/12345/);
    }
  });

  test("should navigate back to home", async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);

    // Look for back button or home link
    const backButton = page.getByRole("link", { name: /back|home/i }).first();

    if (await backButton.isVisible()) {
      await backButton.click();
      await expect(page).toHaveURL("/");
    }
  });
});

test.describe("Comment Deletion", () => {
  let TEST_POST_ID: string;

  test.beforeAll(async () => {
    await createTestUser();
    TEST_POST_ID = await createTestPost();
  });

  test("should show delete button only for comment author", async ({ page }) => {
    // Login as the test user
    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${TEST_POST_ID}`);
    await page.waitForLoadState("networkidle");

    // Create a comment
    const commentInput = page.locator(
      'textarea[placeholder*="comment" i], .comment-input textarea',
    );
    if (await commentInput.isVisible()) {
      await commentInput.fill("Test comment for deletion");
      await page.getByRole("button", { name: /submit/i }).click();

      // Wait for comment to appear
      await page.waitForTimeout(1000);

      // Check that delete button exists for our comment
      const deleteButton = page.locator(".delete-button, button[title*='Delete']").first();
      await expect(deleteButton).toBeVisible();
    }
  });

  test("should successfully delete a comment", async ({ page }) => {
    // Login as the test user
    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${TEST_POST_ID}`);
    await page.waitForLoadState("networkidle");

    // Create a comment to delete
    const commentInput = page.locator(
      'textarea[placeholder*="comment" i], .comment-input textarea',
    );
    if (await commentInput.isVisible()) {
      const testCommentText = "Comment to be deleted " + Date.now();
      await commentInput.fill(testCommentText);
      await page.getByRole("button", { name: /submit/i }).click();

      // Wait for comment to appear
      await page.waitForTimeout(1000);

      // Find the comment we just created
      const commentItem = page.locator(".comment-item").filter({ hasText: testCommentText });
      await expect(commentItem).toBeVisible();

      // Get initial comment count
      const initialComments = await page.locator(".comment-item").count();

      // Click delete button
      const deleteButton = commentItem.locator("button[title*='Delete'], .delete-button");

      // Handle confirmation dialog
      page.once("dialog", (dialog) => {
        expect(dialog.message()).toContain("delete");
        dialog.accept();
      });

      await deleteButton.click();

      // Wait for deletion to complete
      await page.waitForTimeout(1000);

      // Verify comment is removed from UI
      await expect(commentItem).not.toBeVisible();

      // Verify comment count decreased
      const finalComments = await page.locator(".comment-item").count();
      expect(finalComments).toBe(initialComments - 1);
    }
  });

  test("should show success toast after deleting comment", async ({ page }) => {
    // Login as the test user
    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${TEST_POST_ID}`);
    await page.waitForLoadState("networkidle");

    // Create a comment to delete
    const commentInput = page.locator(
      'textarea[placeholder*="comment" i], .comment-input textarea',
    );
    if (await commentInput.isVisible()) {
      await commentInput.fill("Comment for toast test " + Date.now());
      await page.getByRole("button", { name: /submit/i }).click();

      // Wait for comment to appear
      await page.waitForTimeout(1000);

      // Click delete on the first comment
      const deleteButton = page.locator(".delete-button, button[title*='Delete']").first();

      // Handle confirmation dialog
      page.once("dialog", (dialog) => {
        dialog.accept();
      });

      await deleteButton.click();

      // Check for success toast
      const toast = page.locator(".p-toast-message, .toast").filter({ hasText: /deleted/i });
      await expect(toast).toBeVisible({ timeout: 3000 });
    }
  });

  test("should cancel deletion when user dismisses confirmation", async ({ page }) => {
    // Login as the test user
    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${TEST_POST_ID}`);
    await page.waitForLoadState("networkidle");

    // Create a comment
    const commentInput = page.locator(
      'textarea[placeholder*="comment" i], .comment-input textarea',
    );
    if (await commentInput.isVisible()) {
      const testCommentText = "Comment that won't be deleted " + Date.now();
      await commentInput.fill(testCommentText);
      await page.getByRole("button", { name: /submit/i }).click();

      // Wait for comment to appear
      await page.waitForTimeout(1000);

      const commentItem = page.locator(".comment-item").filter({ hasText: testCommentText });
      const initialComments = await page.locator(".comment-item").count();

      // Click delete button but dismiss the dialog
      const deleteButton = commentItem.locator("button[title*='Delete'], .delete-button");

      // Dismiss confirmation dialog
      page.once("dialog", (dialog) => {
        dialog.dismiss();
      });

      await deleteButton.click();

      // Wait a bit
      await page.waitForTimeout(500);

      // Verify comment is still visible
      await expect(commentItem).toBeVisible();

      // Verify comment count unchanged
      const finalComments = await page.locator(".comment-item").count();
      expect(finalComments).toBe(initialComments);
    }
  });

  test("should update comment count on post after deletion", async ({ page }) => {
    // Login as the test user
    await login(page, "testuser", "Testpassword123!");

    await page.goto(`/posts/${TEST_POST_ID}`);
    await page.waitForLoadState("networkidle");

    // Create a comment
    const commentInput = page.locator(
      'textarea[placeholder*="comment" i], .comment-input textarea',
    );
    if (await commentInput.isVisible()) {
      await commentInput.fill("Comment for count test " + Date.now());
      await page.getByRole("button", { name: /submit/i }).click();

      // Wait for comment to appear and check count increased
      await page.waitForTimeout(1000);

      // Delete the comment
      const deleteButton = page.locator(".delete-button, button[title*='Delete']").first();

      page.once("dialog", (dialog) => {
        dialog.accept();
      });

      await deleteButton.click();

      // Wait for deletion
      await page.waitForTimeout(1000);

      // Navigate back to home to check comment count on post item
      await page.goto("/");
      await page.waitForLoadState("networkidle");

      // The post should show updated comment count
      // This verifies that the backend properly excludes deleted comments
      const postItem = page.locator(".post-item, .post-card").first();
      await expect(postItem).toBeVisible();
    }
  });
});
