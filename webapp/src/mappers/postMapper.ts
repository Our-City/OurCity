/*
  /webapp/src/mappers/postMapper.ts

  This file contains the mapping functions to convert between Post DTOs and Post models.
*/
import type { Post } from "@/models/post";
import type { PostResponseDto } from "@/types/dtos/post";
import type { PostCreateRequestDto, PostUpdateRequestDto } from "@/types/dtos/post";
import { toTags } from "./tagMapper";

// DTOs -> Models:
// maps a PostResponseDto to a Post model
export function toPost(dto: PostResponseDto): Post {
  return {
    id: dto.id,
    authorId: dto.authorId,
    title: dto.title,
    description: dto.description,
    location: dto.location,
    authorName: dto.authorName,
    upvoteCount: dto.upvoteCount,
    downvoteCount: dto.downvoteCount,
    voteCount: dto.upvoteCount - dto.downvoteCount,
    commentCount: dto.commentCount,
    visibility: dto.visibility,
    tags: toTags(dto.tags),
    isDeleted: dto.isDeleted,
    voteStatus: dto.voteStatus,
    createdAt: new Date(dto.createdAt),
    updatedAt: new Date(dto.updatedAt),
  };
}
// maps an array of PostResponseDto to an array of Post models
export function toPosts(dtos: PostResponseDto[]): Post[] {
  return dtos.map(toPost);
}

// Models -> DTOs:

// maps a Post model to a PostCreateRequestDto 
export function toPostCreateRequestDto(post: Post): PostCreateRequestDto {
  return {
    title: post.title,
    description: post.description,
    location: post.location,
    tags: post.tags.map(tag => tag.id),
  };
}
// maps a Post model to a PostUpdateRequestDto
export function toPostUpdateRequestDto(post: Post): PostUpdateRequestDto {
  return {
    title: post.title,
    description: post.description,
    location: post.location,
    tags: post.tags.map(tag => tag.id),
  };
}
