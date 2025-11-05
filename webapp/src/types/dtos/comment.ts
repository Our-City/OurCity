/*
  /webapp/src/models/dtos/comment.ts

  This file contains the Data Transfer Objects (DTOs) related to the Comment entity.
  The interfaces corresponding to each DTO are defined as per the request/response definitions in the API contract.
*/

import type { VoteType } from "../enums";

// request DTOs
export interface CommentRequestDto {
  content: string;
};

export interface CommentVoteRequestDto {
  voteType: VoteType;
};

// response DTOs
export interface CommentResponseDto {
  id: string;
  postId: string;
  authorId: string;
  content: string;
  authorName?: string;
  upvoteCount: number;
  downvoteCount: number;
  voteStatus: VoteType;
  isDeleted: boolean;
  createdAt: string; // ISO date string
  updatedAt: string; // ISO date string
};

// TODO: postListResponseDto (unclear on pagination)
export interface CommentListResponseDto {
  comments: CommentResponseDto[];
  nextCursor?: string; // for pagination
}