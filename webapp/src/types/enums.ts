/*
  /webapp/src/types/enums.ts

  Enums used across the application.
*/

export enum PostVisibility {
  PUBLISHED = "Published",
  HIDDEN = "Hidden",
}

export enum VoteType {
  UPVOTE = 1,
  DOWNVOTE = -1,
  NOVOTE = 0,
}

export enum Period {
  Day = "Day",
  Month = "Month",
  Year = "Year",
}
