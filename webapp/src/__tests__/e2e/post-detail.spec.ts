import { test, expect } from '@playwright/test';

test.describe('Post Detail Page', () => {
  // Note: You'll need to use an actual post ID from your test database
  const TEST_POST_ID = '1';

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
    } else {
      // Look for individual vote buttons
      const upvoteButton = page.getByRole('button', { name: /upvote|up/i }).first();
      const downvoteButton = page.getByRole('button', { name: /downvote|down/i }).first();
      
      await expect(upvoteButton).toBeVisible({ timeout: 5000 });
      await expect(downvoteButton).toBeVisible({ timeout: 5000 });
    }
  });

  test('should display comments section', async ({ page }) => {
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Look for comments list
    const commentList = page.locator('[data-testid="comment-list"]');
    await expect(commentList).toBeVisible({ timeout: 10000 });
  });

  test.skip('should allow adding a comment when authenticated', async ({ page }) => {
    // TODO: Add authentication setup
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Look for comment input
    const commentInput = page.locator('[data-testid="comment-input"], textarea, .ql-editor').first();
    await expect(commentInput).toBeVisible({ timeout: 5000 });
    
    // Type a comment
    await commentInput.fill('This is a test comment');
    
    // Submit comment
    const submitButton = page.getByRole('button', { name: /comment|submit|post/i });
    await submitButton.click();
    
    // Wait for comment to appear
    await expect(page.getByText('This is a test comment')).toBeVisible({ timeout: 5000 });
  });

  test.skip('should allow voting on post when authenticated', async ({ page }) => {
    // TODO: Add authentication setup
    await page.goto(`/posts/${TEST_POST_ID}`);
    
    // Get initial vote count
    const voteCount = page.locator('[data-testid="vote-count"]').first();
    const initialCount = await voteCount.textContent();
    
    // Click upvote
    const upvoteButton = page.getByRole('button', { name: /upvote|up/i }).first();
    await upvoteButton.click();
    
    // Wait for vote count to update
    await page.waitForTimeout(1000);
    
    // Check if count changed
    const newCount = await voteCount.textContent();
    expect(newCount).not.toBe(initialCount);
  });

  test('should handle non-existent post gracefully', async ({ page }) => {
    await page.goto('/posts/999999');
    
    // Should show error message or 404
    const errorMessage = page.locator('.error-message, [role="alert"], .p-toast-message-error');
    
    // Either error message is visible or redirected
    try {
      await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
    } catch {
      // Or check if redirected to home
      await expect(page).toHaveURL('/', { timeout: 5000 });
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
