/*
  /webapp/src/mappers/tagMapper.ts

  This file contains the mapping functions to convert between Tag DTOs and Tag models.
*/
import type { TagDto } from "@/types/dtos/tag";
import type { Tag } from "@/models/tag";

// DTOs -> Models:
// maps a TagResponseDto to a Tag model
export function toTag(dto: TagDto): Tag {
  return {
    id: dto.id,
    name: dto.name,
  };
}
// maps an array of TagResponseDto to an array of Tag models
export function toTags(dtos: TagDto[]): Tag[] {
  return dtos.map(toTag);
}
