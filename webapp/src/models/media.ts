/*
  webapp/src/models/media.ts

  This file contains the Media domain model, which represents the Media entity in the application.
*/
export interface Media {
  id: string;
  postId: string;
  url: string;
  createdAt: Date;
  updatedAt: Date;
}