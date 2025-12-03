/// Generative AI - CoPilot was used to assist in the creation of this file.
///   CoPilot was asked to help write e2e tests by being given a description of
///   what should be tested for this and giving back the needed functions
///   and syntax to implement the tests.

/**
 * Helper functions for e2e tests
 */

import { Page } from "@playwright/test";
import { request } from "@playwright/test";

/**
 * Login helper function
 * Use this to authenticate a user before running tests that require auth
 */
export async function login(page: Page, email: string, password: string) {
  await page.goto("/login");
  await page.getByLabel(/email|username/i).fill(email);
  // use input[type="password"] to avoid matching the "Show password" button (was causing some tests to fail)
  await page.locator('input[type="password"]').fill(password);
  await page.getByRole("button", { name: /login|sign in/i }).click();

  // Wait for redirect to home
  await page.waitForURL("/");
}

/**
 * Logout helper function
 */
export async function logout(page: Page) {
  // Adjust this based on your actual logout implementation
  const logoutButton = page.getByRole("button", { name: /logout|sign out/i });

  if (await logoutButton.isVisible()) {
    await logoutButton.click();
  }
}

/**
 * Creates a test user
 */
export async function createTestUser() {
  const api = await request.newContext();
  await api.post("http://localhost:8000/apis/v1/users", {
    data: {
      username: "testuser",
      password: "Testpassword123!",
      passwordConfirm: "Testpassword123!",
    },
  });
}

/**
 * Creates an admin test user (requires the admin user to already exist)
 * This function creates a regular test user and then promotes it to admin
 */
export async function createAdminUser() {
  const api = await request.newContext();
  const adminUsername = "testadmin";
  const adminPassword = "Testpassword123!";

  // Create the test user first
  await api.post("http://localhost:8000/apis/v1/users", {
    data: {
      username: adminUsername,
      password: adminPassword,
      passwordConfirm: adminPassword,
    },
  });

  // Login as the seeded admin user (username: "admin")
  // Password is read from ADMIN_PASSWORD environment variable (loaded from .env.development)
  const adminApi = await request.newContext();
  const seededAdminPassword = process.env.ADMIN_PASSWORD;

  if (!seededAdminPassword) {
    throw new Error(
      "ADMIN_PASSWORD environment variable not found. " +
        "Ensure .env.development file exists with ADMIN_PASSWORD set.",
    );
  }

  const adminLoginResponse = await adminApi.post(
    "http://localhost:8000/apis/v1/authentication/login",
    {
      data: {
        username: "admin",
        password: seededAdminPassword,
      },
    },
  );

  if (!adminLoginResponse.ok()) {
    throw new Error(
      `Failed to login as admin: ${adminLoginResponse.status()} ${await adminLoginResponse.text()}`,
    );
  }

  // Promote the test user to admin
  const promoteResponse = await adminApi.put(
    `http://localhost:8000/apis/v1/admin/users/${adminUsername}/promote-to-admin`,
  );

  if (!promoteResponse.ok()) {
    throw new Error(
      `Failed to promote user to admin: ${promoteResponse.status()} ${await promoteResponse.text()}`,
    );
  }

  await adminApi.dispose();
  await api.dispose();

  return { username: adminUsername, password: adminPassword };
}

/**
 * Create a test post via API with hardcoded data and return its ID (GUID)
 */
export async function createTestPost(overrides?: { title?: string; description?: string }) {
  const api = await request.newContext();
  // Hardcoded test user and post data
  const username = "testuser";
  const password = "Testpassword123!";
  const title = overrides?.title ?? "Test Post Title";
  const description =
    overrides?.description ?? "This is a test post description with sample content.";

  // Authenticate and get token
  await api.post("http://localhost:8000/apis/v1/authentication/login", {
    data: { username, password },
  });

  // Create post without tags (tags need to be GUIDs, send empty array)
  const postRes = await api.post("http://localhost:8000/apis/v1/posts", {
    data: { title, description, tags: [] },
  });
  if (postRes.status() !== 201 && postRes.status() !== 200) {
    throw new Error(`Failed to create post: ${postRes.status()} ${await postRes.text()}`);
  }
  const postJson = await postRes.json();
  return postJson.id || postJson.postId || postJson.guid;
}

/**
 * Wait for API calls to complete
 */
export async function waitForApiResponse(page: Page, urlPattern: string | RegExp) {
  return page.waitForResponse(
    (response) =>
      (typeof urlPattern === "string"
        ? response.url().includes(urlPattern)
        : urlPattern.test(response.url())) && response.status() === 200,
  );
}

/**
 * Create a test post (requires authentication)
 */
export async function createPost(page: Page, title: string, description: string, tags?: string[]) {
  await page.goto("/create-post");

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
        await page.keyboard.press("Enter");
      }
    }
  }

  // Submit
  await page.getByRole("button", { name: /submit|create|post/i }).click();
}

/**
 * Common test data
 */
export const TEST_USERS = {
  valid: {
    email: "test@example.com",
    password: "Test123!@#",
    username: "testuser",
  },
  invalid: {
    email: "invalid@example.com",
    password: "wrongpassword",
  },
};

export const TEST_POSTS = {
  sample: {
    title: "Test Post Title",
    description: "This is a test post description with sample content.",
    tags: ["test", "sample"],
  },
};
