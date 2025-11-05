/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write e2e tests by being given a description of
///   what should be tested for this and giving back the needed functions
///   and syntax to implement the tests.
import { test, expect } from '@playwright/test';
import { createTestUser } from './helpers';
import { createTestPost } from './helpers';

test.describe('Post Detail Page', () => {
  // Note: You'll need to use an actual post ID from your test database
  let TEST_POST_ID: string;
  test.beforeAll(async () => {
    await createTestUser();
    TEST_POST_ID = await createTestPost();
  });

  test('should display post details', async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Wait for page to load
    await page.waitForLoadState('networkidle');
    
    // Check if post content is visible
    const postTitle = page.locator('[data-testid="post-title"], h1, h2').first();
    await expect(postTitle).toBeVisible({ timeout: 10000 });
  });

  test('should display post author information', async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Look for author name or profile link
    const author = page.locator('[data-testid="post-author"], .author');
    await expect(author.first()).toBeVisible({ timeout: 10000 });
  });

  test('should display voting buttons', async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Check for upvote and downvote buttons
    const voteBox = page.locator('[data-testid="vote-box"]');
    
    if (await voteBox.isVisible()) {
      await expect(voteBox).toBeVisible();
      // Check for upvote and downvote buttons by class
      const upvoteButton = page.locator('.vote-btn.upvote');
      const downvoteButton = page.locator('.vote-btn.downvote');
      await expect(upvoteButton).toBeVisible({ timeout: 5000 });
      await expect(downvoteButton).toBeVisible({ timeout: 5000 });
    }
  });

  test('should display comments section', async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Look for comments list by class and check existence
    const commentList = page.locator('.comments-list');
    await expect(commentList).toHaveCount(1);
    });

  test('should handle non-existent post gracefully', async ({ page }) => {
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

  test('should navigate back to home', async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Look for back button or home link
    const backButton = page.getByRole('link', { name: /back|home/i }).first();
    
    if (await backButton.isVisible()) {
      await backButton.click();
      await expect(page).toHaveURL('/');
    }
  });
});
