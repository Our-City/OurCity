/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write e2e tests by being given a description of
///   what should be tested for this and giving back the needed functions
///   and syntax to implement the tests.
import { test, expect } from "@playwright/test";
import { createTestUser } from "./helpers";

test.describe("Authentication Flow", () => {
  test("should navigate to login page", async ({ page }) => {
    await page.goto("/");

    // Navigate to login - adjust selector based on your UI
    await page.goto("/login");

    await expect(page).toHaveURL("/login");
    await expect(page.getByRole("heading", { name: /Welcome Back/i })).toBeVisible();
  });

  test("should display login form with required fields", async ({ page }) => {
    await page.goto("/login");

    // Check for username input
    const usernameInput = page.getByLabel(/username/i);
    await expect(usernameInput).toBeVisible();

    // Check for password input
    const passwordInput = page.locator("#password");
    await expect(passwordInput).toBeVisible();

    // Check for submit button
    const submitButton = page.getByRole("button", { name: /login|sign in/i });
    await expect(submitButton).toBeVisible();
  });

  test("should show validation errors on empty submit", async ({ page }) => {
    await page.goto("/login");

    // Try to submit empty form (simulate user clicking submit)
    const submitButton = page.getByRole("button", { name: /sign in/i });
    await submitButton.click();

    // Check for username error
    const usernameError = page.locator(".form-error", { hasText: "Username is required" });
    await expect(usernameError).toBeVisible({ timeout: 3000 });

    // Check for password error
    const passwordError = page.locator(".form-error", { hasText: "Password is required" });
    await expect(passwordError).toBeVisible({ timeout: 3000 });
  });

  test("should navigate to register page from login", async ({ page }) => {
    await page.goto("/login");

    // Look for link to register page
    const registerLink = page.getByRole("link", { name: /register|sign up|create account/i });

    if (await registerLink.isVisible()) {
      await registerLink.click();
      await expect(page).toHaveURL("/register");
    }
  });

  test("should display register form with required fields", async ({ page }) => {
    await page.goto("/register");

    await expect(page.getByRole("heading", { name: /register|sign up/i })).toBeVisible();

    const usernameInput = page.getByLabel(/username/i);
    await expect(usernameInput).toBeVisible();

    const passwordInput = page.getByLabel(/^password/i).first();
    await expect(passwordInput).toBeVisible();

    const passwordConfirmInput = page.getByLabel(/confirm/i);
    await expect(passwordConfirmInput).toBeVisible();
  });

  test.describe("login with valid credentials", () => {
    test.beforeEach(async () => {
      await createTestUser();
    });

    test("should login successfully with valid credentials", async ({ page }) => {
      await page.goto("/login");

      // Fill in login form with test credentials
      await page.getByLabel(/username/i).fill("testuser");
      await page.locator("#password").fill("Testpassword123!");

      // Submit form
      await page.getByRole("button", { name: /login|sign in/i }).click();

      // Should redirect to home page
      await expect(page).toHaveURL("/");
    });
  });

  test("should show error message on invalid credentials", async ({ page }) => {
    await page.goto("/login");

    // Fill in login form with invalid credentials
    await page.getByLabel(/username/i).fill("invaliduser");
    await page.locator("#password").fill("wrongpassword");

    // Submit form
    await page.getByRole("button", { name: /login|sign in/i }).click();

    // Wait for error message
    const errorMessage = page.locator(
      '.form-error, .p-error, [role="alert"], .error-message, .p-toast-message-error',
    );
    await expect(errorMessage.first()).toBeVisible({ timeout: 5000 });
  });
});
