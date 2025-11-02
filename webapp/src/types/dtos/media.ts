/*
  /webapp/src/models/dtos/media.ts

  This file contains the Data Transfer Objects (DTOs) related to the Media entity.
  The interfaces corresponding to each DTO are defined as per the request/response definitions in the API contract.
*/

export interface MediaResponseDto {
  id: string;
  postId: string;
  url: string;
  createdAt: string; // ISO date string
  updatedAt: string; // ISO date string
}