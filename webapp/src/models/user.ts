/*
  webapp/src/models/user.ts

  This file contains the User domain model, which represents the User entity in the application.
*/
export interface User {
  id: string;
  username: string;
  email?: string;
  posts?: string[]; // store post ids
  savedPosts?: string[];
  comments?: string[]; // store comment ids
  isAdmin: boolean;
  isBanned: boolean;
  createdAt: Date;
  updatedAt: Date;
}
