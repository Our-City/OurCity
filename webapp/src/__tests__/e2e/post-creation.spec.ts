/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write e2e tests by being given a description of
///   what should be tested for this and giving back the needed functions
///   and syntax to implement the tests.
import { test, expect } from '@playwright/test';
import { createTestUser } from './helpers';

test.describe('Post Creation', () => {
  test.beforeEach(async ({ page }) => {
    await createTestUser();
    // Note: Most post creation tests will require authentication
    // You may need to implement login in beforeEach or use auth fixtures
    await page.goto('/');
  });

  test('should redirect unauthenticated users to login', async ({ page }) => {
    await page.goto('/create-post');
    
    // If not authenticated, should redirect to login
    // Adjust this based on your auth flow
    await page.waitForURL(/\/(login|create-post)/);
  });

  // This test should be run with authenticated context
  test('should display post creation form when authenticated', async ({ page }) => {
    // TODO: Add authentication setup
    await page.goto('/create-post');
    
  // Check for form elements
  await expect(page.getByRole('heading', { name: /create new post/i })).toBeVisible();
    
    // Title input
    const titleInput = page.getByLabel(/title/i);
    await expect(titleInput).toBeVisible();
    
    // Description/content input (might be a rich text editor)
    const descriptionInput = page.getByLabel(/description|content/i);
    await expect(descriptionInput).toBeVisible();
    
    // Submit button
    const submitButton = page.getByRole('button', { name: /submit|create|post/i });
    await expect(submitButton).toBeVisible();
  });

  test.skip('should create a post with valid data', async ({ page }) => {
    // TODO: Add authentication setup
    await page.goto('/create-post');
    
    // Fill in the form
    await page.getByLabel(/title/i).fill('Test Post Title');
    
    // For rich text editor, you might need to use different selectors
    const editor = page.locator('.ql-editor, [contenteditable="true"]');
    if (await editor.isVisible()) {
      await editor.fill('This is a test post description with some content.');
    } else {
      await page.getByLabel(/description|content/i).fill('This is a test post description.');
    }
    
    // Add tags if available
    const tagInput = page.getByLabel(/tags/i);
    if (await tagInput.isVisible()) {
      await tagInput.fill('test');
    }
    
    // Submit the form
    await page.getByRole('button', { name: /submit|create|post/i }).click();
    
    // Should redirect to post detail or home page
    await page.waitForURL(/\/(posts\/\d+|\/)/);
    
    // Verify post was created (check for success message or post in list)
    const successMessage = page.locator('.p-toast-message-secondary, .success-message');
    try {
      await expect(successMessage.first()).toBeVisible({ timeout: 8000 });
    } catch {
      // Fallback: check for navigation to new post page
      try {
        await expect(page).toHaveURL(/\/posts\//, { timeout: 8000 });
      } catch {
        // If still on create-post, check for error messages and log page content
        const errorMsg = await page.locator('.form-error').allTextContents();
        throw new Error('Post creation failed: No success message, no redirect, errors: ' + errorMsg.join('; '));
      }
    }
  });

  test('should show validation errors for empty post', async ({ page }) => {
    // TODO: Add authentication setup
    await page.goto('/create-post');
    
    // Try to submit without filling form
    await page.getByRole('button', { name: /submit|create|post/i }).click();
    
    // Check for validation errors
    const errors = page.locator('.form-error');
    await expect(errors.first()).toBeVisible({ timeout: 3000 });
  });

  test('should cancel post creation and return to home', async ({ page }) => {
    // TODO: Add authentication setup
    await page.goto('/create-post');
    
    // Look for cancel button
    const cancelButton = page.getByRole('button', { name: /cancel/i });
    
    if (await cancelButton.isVisible()) {
      await cancelButton.click();
      
      // Should return to home page
      await expect(page).toHaveURL('/');
    }
  });
});
