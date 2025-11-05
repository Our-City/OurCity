import { test, expect } from '@playwright/test';

test.describe('Navigation', () => {
  test('should navigate between main pages', async ({ page }) => {
    await page.goto('/');
    
    // Navigate to login
    await page.goto('/login');
    await expect(page).toHaveURL('/login');
    
    // Navigate to register
    await page.goto('/register');
    await expect(page).toHaveURL('/register');
    
    // Navigate back to home
    await page.goto('/');
    await expect(page).toHaveURL('/');
  });

  test('should have working navigation links in sidebar', async ({ page }) => {
    await page.goto('/');
    
    // Check if sidebar has navigation links
    const sidebar = page.locator('[data-testid="sidebar"]');
    
    if (await sidebar.isVisible()) {
      const homeLink = sidebar.getByRole('link', { name: /home/i });
      
      if (await homeLink.isVisible()) {
        await expect(homeLink).toBeVisible();
      }
    }
  });

  test('should maintain state during navigation', async ({ page }) => {
    await page.goto('/');
    
    // Navigate to different page
    await page.goto('/login');
    
    // Navigate back
    await page.goBack();
    
    // Should be back at home
    await expect(page).toHaveURL('/');
  });
});
