/**
 * Helper functions for e2e tests
 */

import { Page } from '@playwright/test';

/**
 * Login helper function
 * Use this to authenticate a user before running tests that require auth
 */
export async function login(page: Page, email: string, password: string) {
  await page.goto('/login');
  await page.getByLabel(/email|username/i).fill(email);
  await page.getByLabel(/password/i).fill(password);
  await page.getByRole('button', { name: /login|sign in/i }).click();
  
  // Wait for redirect to home
  await page.waitForURL('/');
}

/**
 * Logout helper function
 */
export async function logout(page: Page) {
  // Adjust this based on your actual logout implementation
  const logoutButton = page.getByRole('button', { name: /logout|sign out/i });
  
  if (await logoutButton.isVisible()) {
    await logoutButton.click();
  }
}

/**
 * Wait for API calls to complete
 */
export async function waitForApiResponse(page: Page, urlPattern: string | RegExp) {
  return page.waitForResponse(response => 
    (typeof urlPattern === 'string' 
      ? response.url().includes(urlPattern)
      : urlPattern.test(response.url())
    ) && response.status() === 200
  );
}

/**
 * Create a test post (requires authentication)
 */
export async function createPost(
  page: Page, 
  title: string, 
  description: string, 
  tags?: string[]
) {
  await page.goto('/create-post');
  
  await page.getByLabel(/title/i).fill(title);
  
  // Handle rich text editor
  const editor = page.locator('.ql-editor, [contenteditable="true"]');
  if (await editor.isVisible()) {
    await editor.fill(description);
  } else {
    await page.getByLabel(/description|content/i).fill(description);
  }
  
  // Add tags if provided
  if (tags && tags.length > 0) {
    const tagInput = page.getByLabel(/tags/i);
    if (await tagInput.isVisible()) {
      for (const tag of tags) {
        await tagInput.fill(tag);
        await page.keyboard.press('Enter');
      }
    }
  }
  
  // Submit
  await page.getByRole('button', { name: /submit|create|post/i }).click();
}

/**
 * Common test data
 */
export const TEST_USERS = {
  valid: {
    email: 'test@example.com',
    password: 'Test123!@#',
    username: 'testuser'
  },
  invalid: {
    email: 'invalid@example.com',
    password: 'wrongpassword'
  }
};

export const TEST_POSTS = {
  sample: {
    title: 'Test Post Title',
    description: 'This is a test post description with sample content.',
    tags: ['test', 'sample']
  }
};
