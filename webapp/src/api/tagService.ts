/*
  /webapp/src/api/tagService.ts

  This file contains the service functions to interact with the Tag related API endpoints.
*/
import api from "./axios";
import type { TagDto } from "@/types/dtos/tag";
import type { Tag } from "@/models/tag";
import { toTags } from "@/mappers/tagMapper";

// GET /tags
export async function getTags(): Promise<Tag[]> {
  const response = await api.get<TagDto[]>("/tags");
  return toTags(response.data);
}
