# E2E Tests with Playwright

This directory contains end-to-end tests for the OurCity web application.

## Running Tests

```bash
# Run all e2e tests
npm run test:e2e

# Run tests in UI mode (interactive)
npm run test:e2e:ui

# Run tests in headed mode (see browser)
npm run test:e2e:headed

# Debug tests
npm run test:e2e:debug

# View test report
npm run test:e2e:report
```

## Test Structure

- `src/__tests__/e2e/` - Test files
  - `home.spec.ts` - Tests for the home page
  - `authentication.spec.ts` - Tests for login and registration
  - `post-creation.spec.ts` - Tests for creating posts
  - `post-detail.spec.ts` - Tests for viewing post details
  - `navigation.spec.ts` - Tests for navigation flows
  - `fixtures/` - Shared test fixtures and helpers
    - `auth.ts` - Authentication fixtures

## Adding Tests

1. Create a new `.spec.ts` file in the `src/__tests__/e2e/` directory
2. Import the test framework: `import { test, expect } from '@playwright/test';`
3. Write your test cases using `test.describe()` and `test()`
4. Use data-testid attributes in components for reliable selectors

## Best Practices

1. **Use data-testid attributes** in your Vue components for stable selectors:
   ```vue
   <div data-testid="post-item">...</div>
   ```

2. **Wait for elements properly**:
   ```typescript
   await expect(element).toBeVisible({ timeout: 5000 });
   ```

3. **Handle authentication** using fixtures or before hooks

4. **Skip tests** that require authentication with `test.skip()` until auth is set up:
   ```typescript
   test.skip('authenticated test', async ({ page }) => {
     // TODO: Add authentication setup
   });
   ```

5. **Use meaningful test names** that describe what is being tested

## Configuration

Test configuration is in `playwright.config.ts` at the project root. Key settings:

- **baseURL**: `http://localhost:5173`
- **Browsers**: Chromium, Firefox, WebKit
- **Web Server**: Automatically starts dev server before tests

## CI/CD

Tests are configured to:
- Retry failed tests 2 times on CI
- Run in a single worker on CI for stability
- Capture traces on first retry
- Take screenshots on failure

## Next Steps

1. Add `data-testid` attributes to your Vue components
2. Uncomment and update the skipped tests once authentication is set up
3. Create test fixtures for common scenarios (authenticated user, test data, etc.)
4. Add visual regression tests if needed
5. Configure test data and mock API responses as needed
