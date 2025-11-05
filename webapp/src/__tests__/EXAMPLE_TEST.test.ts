// dummy test file, needs to be removed
import { describe, it, expect } from 'vitest';

describe('Dummy Test Suite', () => {
  it('should pass a basic truthy test', () => {
    expect(true).toBe(true);
  });

  it('should add two numbers correctly', () => {
    const sum = 1 + 2;
    expect(sum).toBe(3);
  });
});