/*
  /webapp/src/api/postService.ts

  This file contains the service functions to interact with the Post related API endpoints.

  AI assisted in the creation of this file. 
    Specifically, ChatGPT 5.0 was prompted to assist with handling the paginated results.
*/
import api from "./axios"
import type { Comment } from "@/models/comment"
import type { CommentRequestDto, CommentResponseDto, CommentVoteRequestDto } from "../types/dtos/comment"
import { toComment, toComments } from "@/mappers/commentMapper";
import { toCommentRequestDto } from "@/mappers/commentMapper";
import type { PaginatedResult } from "@/types/common/paginatedResult";

// GET /posts/{postId}/comments?limit=&cursor=
export async function getCommentsByPostId(
  postId: string,
  limit = 25,
  cursor?: string | null
): Promise<PaginatedResult<Comment>> {
  const params = new URLSearchParams({ limit: limit.toString() });
  if (cursor) params.append("cursor", cursor);

  const response = await api.get<{ items: CommentResponseDto[]; nextCursor?: string }>(
    `/posts/${postId}/comments?${params.toString()}`
  );

  return {
    items: toComments(response.data.items),
    nextCursor: response.data.nextCursor,
  };
}

// POST /posts/{postId}/comments
export async function createComment(postId: string, comment: Comment): Promise<Comment> {
  const dto: CommentRequestDto = toCommentRequestDto(comment);
  const response = await api.post<CommentResponseDto>(`/posts/${postId}/comments`, dto);

  return toComment(response.data);
}

// PUT /comments/{commentId}
export async function updateComment(commentId: string, comment: Comment): Promise<Comment> {
  const dto: CommentRequestDto = toCommentRequestDto(comment);
  const response = await api.put<CommentResponseDto>(`/comments/${commentId}`, dto);

  return toComment(response.data);
}

// DELETE /comments/{commentId}
export async function deleteComment(commentId: string): Promise<Comment> {
  const response = await api.delete<CommentResponseDto>(`/comments/${commentId}`);
  return toComment(response.data);
}

// PUT /comments/{commentId}/vote
export async function voteOnComment(commentId: string, voteType: CommentVoteRequestDto): Promise<Comment> {
  const response = await api.put<CommentResponseDto>(`/comments/${commentId}/votes`, voteType);
  return toComment(response.data);
}