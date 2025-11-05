/*
  /webapp/src/models/common/PaginatedResult.ts

  This file contains a generic interface for paginated results.
  Intended use is to standardize paginated responses from the API (e.g., lists of posts, comments, etc.)
*/

export interface PaginatedResult<T> {
  items: T[];
  nextCursor?: string;
}
