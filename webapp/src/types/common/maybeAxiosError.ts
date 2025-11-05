/*
  /webapp/src/models/common/PaginatedResult.ts

  This file contains a generic interface for handling potential Axios error objects.

  This file was created with the assistance of AI.
  Specifically, Copilot helped to define the MaybeAxiosError interface.
*/
export interface MaybeAxiosError {
  message?: unknown;
  response?: {
    data?: Record<string, unknown> | undefined;
  };
}
