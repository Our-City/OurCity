import { fileURLToPath } from "node:url";
import { mergeConfig, defineConfig, configDefaults } from "vitest/config";
import viteConfig from "./vite.config";

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: "jsdom",
      exclude: [...configDefaults.exclude, "e2e/**", "src/__tests__/e2e/**"],
      root: fileURLToPath(new URL("./", import.meta.url)),
      setupFiles: ["./src/__tests__/setupTests.ts"],
      // Coverage configuration: include only source files and exclude test helpers,
      // typings and DTO/type-only files which should not be instrumented.
      coverage: {
        provider: "v8",
        reporter: ["text", "lcov", "html"],
        all: true,
        include: ["src/**/*.ts", "src/**/*.vue", "src/**/*.js"],
        exclude: [
          "**/__tests__/**",
          "**/__mocks__/**",
          "**/*.d.ts",
          "src/types/**",
          "src/types/**/**",
          "src/**/dtos/**",
          "src/**/dtos/**/**",
          "src/env.d.ts",
          "src/shims-vue.d.ts",
        ],
      },
    },
  }),
);
