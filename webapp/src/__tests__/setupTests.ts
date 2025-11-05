import { beforeAll } from "vitest";
import { createPinia, setActivePinia } from "pinia";

// make Pinia store available in all tests
beforeAll(() => {
  setActivePinia(createPinia());
});
