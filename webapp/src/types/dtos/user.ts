/*
  /webapp/src/models/dtos/user.ts

  This file contains the Data Transfer Objects (DTOs) related to the User entity.
  The interfaces corresponding to each DTO are defined as per the request/response definitions in the API contract.
*/
import { VoteType } from "../enums";

// request DTOs:
export interface UserCreateRequestDto {
  username: string;
  password: string;
}

export interface UserUpdateRequestDto {
  username?: string;
}

export interface UserVoteRequestDto {
  // TODO: kind of confused by this DTO, need to clarify.
  commentIds: string[];
}

// response DTOs:
export interface UserResponseDto {
  id: string;
  username: string;
  postIds: string[];
  commentIds: string[];
  isAdmin: boolean;
  isBanned: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface UserVoteResponseDto {
  itemId: string;
  vote: VoteType;
}

export interface UserListVoteResponseDto {
  votes: UserVoteResponseDto[];
  nextCursor?: string; // for pagination
}

export interface UserReportResponseDto {
  reason?: string;
}

