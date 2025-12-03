/*
  /webapp/src/models/post.ts

  This file contains the User domain model, which represents the User entity in the application.
*/
import type { Tag } from "./tag";
import { PostVisibility, VoteType } from "../types/enums";

// Post model representing the Post entity in the application
export interface Post {
  id: string;
  authorId: string;
  title: string;
  description: string;
  authorName?: string;
  location?: string;
  latitude?: number;
  longitude?: number;
  upvoteCount: number;
  downvoteCount: number;
  voteCount: number; // total votes (upvotes - downvotes)
  commentCount: number;
  visibility: PostVisibility; // PUBLISHED | HIDDEN
  tags: Tag[];
  isDeleted: boolean;
  canMutate: boolean; // authorization flag from backend
  voteStatus: VoteType; // UPVOTE = 1| DOWNVOTE = -1| NONE = 0
  isBookmarked: boolean;
  isReported: boolean;
  createdAt: Date;
  updatedAt: Date;
}
