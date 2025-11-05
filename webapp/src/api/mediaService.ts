/*
  /webapp/src/api/postService.ts

  This file contains the service functions to interact with the Media related API endpoints.
*/
import api from "@/api/axios";
import type { MediaResponseDto } from "@/types/dtos/media";
import type { Media } from "@/models/media";
import { toMedia } from "@/mappers/mediaMapper";


// POST /media/{postId}
export async function uploadMedia(file: File, postId: string): Promise<Media> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await api.post<MediaResponseDto>(`media/${postId}`, formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
  
  return toMedia(response.data);
}

// GET /media/{mediaId}
export async function getMediaById(mediaId: string): Promise<Media> {
  const response = await api.get<MediaResponseDto>(`/media/${mediaId}`);
  return toMedia(response.data);
}

// GET /media/post/{postId}
export async function getMediaByPostId(postId: string): Promise<Media[]> {
  const response = await api.get<MediaResponseDto[]>(`/media/posts/${postId}`);
  return response.data.map(toMedia);
}

// DELETE /media/{mediaId}
export async function deleteMedia(mediaId: string): Promise<void> {
  await api.delete(`/media/${mediaId}`);
}