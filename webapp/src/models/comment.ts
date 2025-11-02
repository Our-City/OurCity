/*
  /webapp/src/models/comment.ts

  This file contains the Comment domain model, which represents the Comment entity in the application.
*/
import { VoteType } from "../types/enums";

// Post model representing the Post entity in the application
export interface Comment {
  id: string;
  postID: string;
  authorID: string;
  content: string;
  upvoteCount: number;
  downvoteCount: number;
  voteCount: number; // total votes (upvotes - downvotes)
  voteStatus: VoteType; // UPVOTE = 1| DOWNVOTE = -1| NONE = 0
  isDeleted: boolean;
  createdAt: Date;
  updatedAt: Date;
}