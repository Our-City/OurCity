/*
  /webapp/src/mappers/mediaMapper.ts

  This file contains the mapping functions to convert between Media DTOs and Media models.
*/
import type { MediaResponseDto } from "@/types/dtos/media";

// DTOs -> Models:
// map MediaResponseDto to Media model
export function toMedia(dto: MediaResponseDto) {
  return {
    id: dto.id,
    postId: dto.postId,
    url: dto.url,
    createdAt: new Date(dto.createdAt),
    updatedAt: new Date(dto.updatedAt),
  };
}
