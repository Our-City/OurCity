/*
  /webapp/src/api/postService.ts

  This file contains the service functions to interact with the Post related API endpoints.

  AI assisted in the creation of this file. 
    Specifically, ChatGPT 5.0 was prompted to assist with handling the paginated results.
*/
import api from "./axios"
import type { Post } from "@/models/post"
import type { PostResponseDto, PostCreateRequestDto, PostUpdateRequestDto } from "@/types/dtos/post"
import { toPost, toPosts } from "@/mappers/postMapper"
import { toPostCreateRequestDto, toPostUpdateRequestDto } from "@/mappers/postMapper"
import type { PaginatedResult } from "@/types/common/paginatedResult";


// GET /posts/{postId}
export async function getPostById(postId: string): Promise<Post> {
  const response = await api.get<PostResponseDto>(`/posts/${postId}`);
  return toPost(response.data);
}

// GET /posts?limit=&cursor=
export async function getPosts(
  limit = 25,
  cursor?: string | null
): Promise<PaginatedResult<Post>> {
  const params = new URLSearchParams({ limit: limit.toString() });
  if (cursor) params.append("cursor", cursor);

  // backend returns { items, nextCursor }
  const response = await api.get<{ items: PostResponseDto[]; nextCursor?: string }>(
    `/posts?${params.toString()}`
  );

  return {
    items: toPosts(response.data.items),
    nextCursor: response.data.nextCursor,
  };
}

// POST /posts
export async function createPost(post: Post): Promise<Post> {
  const dto: PostCreateRequestDto = toPostCreateRequestDto(post);
  const response = await api.post<PostResponseDto>("/posts", dto);
  return toPost(response.data);
}

// PUT /posts/{postId}
export async function updatePost(postId: string, post: Post): Promise<Post> {
  const dto: PostUpdateRequestDto = toPostUpdateRequestDto(post);
  const response = await api.put<PostResponseDto>(`/posts/${postId}`, dto);
  return toPost(response.data);
}

// DELETE /posts/{postId}
export async function deletePost(postId: string): Promise<Post> {
  const response = await api.delete<PostResponseDto>(`/posts/${postId}`);
  return toPost(response.data);
}

// PUT /posts/{postId}/vote
export async function voteOnPost(postId: string, voteType: number): Promise<Post> {
  const response = await api.put<PostResponseDto>(`/posts/${postId}/votes`, { voteType });
  return toPost(response.data);
}