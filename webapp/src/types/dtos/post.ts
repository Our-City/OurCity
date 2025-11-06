/*
  /webapp/src/models/dtos/post.ts

  This file contains the Data Transfer Objects (DTOs) related to the Post entity.
  The interfaces corresponding to each DTO are defined as per the request/response definitions in the API contract.
*/

import type { PostVisibility, VoteType } from "../enums";
import type { TagDto } from "./tag";

// response DTOs for Post
export interface PostResponseDto {
  id: string;
  authorId: string;
  title: string;
  description: string;
  authorName?: string;
  location?: string;
  upvoteCount: number;
  downvoteCount: number;
  commentCount: number;
  visibility: PostVisibility;
  tags: TagDto[];
  isDeleted: boolean;
  voteStatus: VoteType;
  createdAt: string; // ISO date string
  updatedAt: string; // ISO date string
}

// TODO: postListResponseDto (unclear on pagination)
export interface PostListResponseDto {
  posts: PostResponseDto[];
  nextCursor?: string; // for pagination
}

// request DTOs for Post
export interface PostGetAllRequestDto {
  cursor?: string;
  limit?: number;
  searchTerm?: string;
  tags?: string[]; // array of tag IDs
  sortBy?: string;
  sortOrder?: "Asc" | "Desc";
}

export interface PostCreateRequestDto {
  title: string;
  description: string;
  location?: string;
  tags?: string[]; // array of tag IDs
}

export interface PostUpdateRequestDto {
  title?: string;
  description?: string;
  location?: string;
  tags?: string[]; // array of tag IDs
}

export interface PostVoteRequestDto {
  voteType: VoteType; // UPVOTE = 1| DOWNVOTE = -1| NONE = 0
}
