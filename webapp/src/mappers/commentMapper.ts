/*
  /webapp/src/mappers/commentMapper.ts

  This file contains the mapping functions to convert between Comment DTOs and Post models.
*/
import type { Comment } from "@/models/comment";
import type { CommentResponseDto } from "@/types/dtos/comment";
import type { CommentRequestDto } from "@/types/dtos/comment";
import { parseVoteType } from "@/utils/voteUtils";

// DTOs -> Models:
// maps a CommentResponseDto to a Comment model
export function toComment(dto: CommentResponseDto): Comment {
  return {
    id: dto.id,
    postId: dto.postId,
    authorId: dto.authorId,
    content: dto.content,
    authorName: dto.authorName,
    upvoteCount: dto.upvoteCount,
    downvoteCount: dto.downvoteCount,
    voteCount: dto.upvoteCount - dto.downvoteCount,
    voteStatus: parseVoteType(dto.voteStatus),
    isDeleted: dto.isDeleted,
    createdAt: new Date(dto.createdAt),
    updatedAt: new Date(dto.updatedAt),
  };
}
// maps an array of CommentResponseDto to an array of Comment models
export function toComments(dtos: CommentResponseDto[]): Comment[] {
  return dtos.map(toComment);
}

// Models -> DTOs:
// maps a Comment model to a CommentRequestDto
export function toCommentRequestDto(comment: Comment): CommentRequestDto {
  return {
    content: comment.content,
  };
}
