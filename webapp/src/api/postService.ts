/*
  /webapp/src/api/postService.ts

  This file contains the service functions to interact with the Post related API endpoints.

  AI assisted in the creation of this file. 
    Specifically, ChatGPT 5.0 was prompted to assist with handling the paginated results.
*/
import api from "./axios";
import type { Post } from "@/models/post";
import type {
  PostResponseDto,
  PostCreateRequestDto,
  PostUpdateRequestDto,
  PostGetAllRequestDto,
} from "@/types/dtos/post";
import type { VoteType } from "@/types/enums";
import { toPost, toPosts } from "@/mappers/postMapper";
import { toPostCreateRequestDto, toPostUpdateRequestDto } from "@/mappers/postMapper";
import type { PaginatedResult } from "@/types/common/paginatedResult";

// GET /posts/{postId}
export async function getPostById(postId: string): Promise<Post> {
  const response = await api.get<PostResponseDto>(`/posts/${postId}`);
  return toPost(response.data);
}

// GET /posts?limit=&cursor=
export async function getPosts(params: PostGetAllRequestDto): Promise<PaginatedResult<Post>> {
  const searchParams = new URLSearchParams();

  if (params.limit) searchParams.append("limit", params.limit.toString());
  if (params.cursor) searchParams.append("cursor", params.cursor);
  if (params.searchTerm) searchParams.append("searchTerm", params.searchTerm);
  if (params.tags && params.tags.length > 0)
    params.tags.forEach((tag) => searchParams.append("tags", tag));
  if (params.sortBy) searchParams.append("sortBy", params.sortBy);
  if (params.sortOrder) searchParams.append("sortOrder", params.sortOrder);

  const response = await api.get<{ items: PostResponseDto[]; nextCursor?: string }>(
    `/posts?${searchParams.toString()}`,
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
export async function voteOnPost(postId: string, voteType: VoteType): Promise<Post> {
  const response = await api.put<PostResponseDto>(`/posts/${postId}/votes`, { voteType });
  return toPost(response.data);
}

// PUT posts/{postId}/bookmarks
export async function bookmarkPost(postId: string): Promise<Post> {
  const response = await api.put<PostResponseDto>(`/posts/${postId}/bookmarks`);
  return toPost(response.data);
}

// GET /posts/bookmarks?limit=&cursor=
export async function getBookmarks(params: PostGetAllRequestDto): Promise<PaginatedResult<Post>> {
  const searchParams = new URLSearchParams();

  if (params.limit) searchParams.append("limit", params.limit.toString());
  if (params.cursor) searchParams.append("cursor", params.cursor);
  if (params.searchTerm) searchParams.append("searchTerm", params.searchTerm);
  if (params.tags && params.tags.length > 0)
    params.tags.forEach((tag) => searchParams.append("tags", tag));
  if (params.sortBy) searchParams.append("sortBy", params.sortBy);
  if (params.sortOrder) searchParams.append("sortOrder", params.sortOrder);

  const response = await api.get<{ items: PostResponseDto[]; nextCursor?: string }>(
    `/posts/bookmarks?${searchParams.toString()}`,
  );

  return {
    items: toPosts(response.data.items),
    nextCursor: response.data.nextCursor,
  };
}