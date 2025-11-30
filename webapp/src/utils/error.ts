/*
  /webapp/src/utils/error.ts

  Utility helpers for working with error objects returned from API calls or other async operations.

  AI assisted in the creation of this file.
  Specifically, Copilot helped to write the resolveErrorMessage function.
*/
import type { MaybeAxiosError } from "@/types/common/maybeAxiosError";

/**
 * Formats comma-separated error messages into a readable multi-line format
 */
function formatErrorMessage(message: string): string {
  // Check if the message contains comma-separated sentences
  if (message.includes(".,")) {
    return message
      .split(".,")
      .map((msg) => msg.trim())
      .filter((msg) => msg.length > 0)
      .map((msg) => (msg.endsWith(".") ? msg : msg + "."))
      .join("<br>");
  }
  return message;
}

/**
 * Attempts to extract a meaningful error message from an unknown error value.
 * Falls back to the provided default when no message can be found.
 */
export function resolveErrorMessage(error: unknown, fallbackMessage: string): string {
  if (typeof error === "string" && error.trim()) {
    return formatErrorMessage(error);
  }

  if (error instanceof Error) {
    const message = error.message?.trim();
    if (message) {
      return formatErrorMessage(message);
    }
  }

  if (typeof error === "object" && error !== null) {
    const maybeAxiosError = error as MaybeAxiosError;
    const responseData = maybeAxiosError.response?.data;

    if (responseData && typeof responseData === "object") {
      const candidates = [
        (responseData as { error?: unknown }).error,
        (responseData as { detail?: unknown }).detail,
        (responseData as { message?: unknown }).message,
        (responseData as { title?: unknown }).title,
      ];

      for (const candidate of candidates) {
        if (typeof candidate === "string") {
          const trimmed = candidate.trim();
          if (trimmed) {
            return formatErrorMessage(trimmed);
          }
        }
      }
    }

    if (typeof maybeAxiosError.message === "string" && maybeAxiosError.message.trim()) {
      return formatErrorMessage(maybeAxiosError.message.trim());
    }
  }

  return fallbackMessage;
}
