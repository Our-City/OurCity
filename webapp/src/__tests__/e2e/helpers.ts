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
  await page.getByLabel(/password/i).fill(password);
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
  const response = await api.post("http://localhost:8000/apis/v1/users", {
    data: {
      username: "testuser",
      password: "Testpassword123!",
      passwordConfirm: "Testpassword123!",
    },
  });
}

/**
 * Create a test post via API with hardcoded data and return its ID (GUID)
 */
export async function createTestPost() {
  const api = await request.newContext();
  // Hardcoded test user and post data
  const username = "testuser";
  const password = "Testpassword123!";
  const title = "Test Post Title";
  const description = "This is a test post description with sample content.";
  const tags = ["test", "sample"];

  // Authenticate and get token
  const loginRes = await api.post("http://localhost:8000/apis/v1/authentication/login", {
    data: { username, password },
  });
  // // if (loginRes.status() !== 200) {
  // //   const errorText = await loginRes.text();
  // //   throw new Error(`Login failed: ${loginRes.status()} - ${errorText}`);
  // // }
  // let loginJson;
  // try {
  //   loginJson = await loginRes.json();
  // } catch (e) {
  //   const errorText = await loginRes.text();
  //   throw new Error(`Login response not JSON: ${errorText}`);
  // }
  // const token = loginJson?.token || loginJson?.accessToken;
  // if (!token) throw new Error('Failed to get auth token for post creation');

  // Create post
  const postRes = await api.post("http://localhost:8000/apis/v1/posts", {
    data: { title, description, tags },
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
