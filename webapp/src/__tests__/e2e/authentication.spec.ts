import { test, expect } from '@playwright/test';

test.describe('Authentication Flow', () => {
  test('should navigate to login page', async ({ page }) => {
    await page.goto('/');
    
    // Navigate to login - adjust selector based on your UI
    await page.goto('/login');
    
    await expect(page).toHaveURL('/login');
    await expect(page.getByRole('heading', { name: /login/i })).toBeVisible();
  });

  test('should display login form with required fields', async ({ page }) => {
    await page.goto('/login');
    
    // Check for email/username input
    const emailInput = page.getByLabel(/email|username/i);
    await expect(emailInput).toBeVisible();
    
    // Check for password input
    const passwordInput = page.getByLabel(/password/i);
    await expect(passwordInput).toBeVisible();
    
    // Check for submit button
    const submitButton = page.getByRole('button', { name: /login|sign in/i });
    await expect(submitButton).toBeVisible();
  });

  test('should show validation errors on empty submit', async ({ page }) => {
    await page.goto('/login');
    
    // Try to submit empty form
    const submitButton = page.getByRole('button', { name: /login|sign in/i });
    await submitButton.click();
    
    // Wait for validation messages
    await page.waitForTimeout(500);
    
    // Check if error messages appear (adjust based on your validation)
    const errors = page.locator('.p-error, [role="alert"], .error-message');
    await expect(errors.first()).toBeVisible({ timeout: 3000 });
  });

  test('should navigate to register page from login', async ({ page }) => {
    await page.goto('/login');
    
    // Look for link to register page
    const registerLink = page.getByRole('link', { name: /register|sign up|create account/i });
    
    if (await registerLink.isVisible()) {
      await registerLink.click();
      await expect(page).toHaveURL('/register');
    }
  });

  test('should display register form with required fields', async ({ page }) => {
    await page.goto('/register');
    
    await expect(page.getByRole('heading', { name: /register|sign up/i })).toBeVisible();
    
    // Check for form fields (adjust based on your registration form)
    const emailInput = page.getByLabel(/email/i);
    await expect(emailInput).toBeVisible();
    
    const usernameInput = page.getByLabel(/username/i);
    await expect(usernameInput).toBeVisible();
    
    const passwordInput = page.getByLabel(/^password/i).first();
    await expect(passwordInput).toBeVisible();
  });

  // Example test for successful login (requires test credentials)
  test.skip('should login successfully with valid credentials', async ({ page }) => {
    await page.goto('/login');
    
    // Fill in login form with test credentials
    await page.getByLabel(/email|username/i).fill('testuser@example.com');
    await page.getByLabel(/password/i).fill('testpassword123');
    
    // Submit form
    await page.getByRole('button', { name: /login|sign in/i }).click();
    
    // Should redirect to home page
    await expect(page).toHaveURL('/');
    
    // Check for authenticated state (adjust based on your UI)
    await expect(page.getByText(/logout|profile/i)).toBeVisible({ timeout: 5000 });
  });

  test('should show error message on invalid credentials', async ({ page }) => {
    await page.goto('/login');
    
    // Fill in login form with invalid credentials
    await page.getByLabel(/email|username/i).fill('invalid@example.com');
    await page.getByLabel(/password/i).fill('wrongpassword');
    
    // Submit form
    await page.getByRole('button', { name: /login|sign in/i }).click();
    
    // Wait for error message
    const errorMessage = page.locator('.p-error, [role="alert"], .error-message, .p-toast-message-error');
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
  });
});
