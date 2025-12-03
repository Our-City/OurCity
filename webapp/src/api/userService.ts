/*
  /webapp/src/api/userService.ts

  This file contains the service functions to interact with the User and Me API endpoints.
*/
import api from "./axios";
import type { User } from "@/models/user";
import type {
  UserResponseDto,
  UserCreateRequestDto,
  UserUpdateRequestDto,
} from "@/types/dtos/user";
import { toUser, toUserUpdateRequestDto } from "@/mappers/userMapper";

// POST /users
export async function createUser(username: string, password: string): Promise<User> {
  const dto: UserCreateRequestDto = {
    username,
    password,
  };
  const response = await api.post<UserResponseDto>("/users", dto);
  return toUser(response.data);
}

// PUT /users/{userId}
export async function updateUser(userId: string, user: Partial<User>): Promise<User> {
  const dto: UserUpdateRequestDto = toUserUpdateRequestDto(user);
  const response = await api.put<UserResponseDto>(`/users/${userId}`, dto);
  return toUser(response.data);
}

// DELETE /users/{userId}
export async function deleteUser(userId: string): Promise<User> {
  const response = await api.delete<UserResponseDto>(`/users/${userId}`);
  return toUser(response.data);
}

// /me endpoints

// GET /me
export async function getCurrentUser(): Promise<User> {
  const response = await api.get<UserResponseDto>("/me");
  return toUser(response.data);
}

// PUT /me
export async function updateCurrentUser(user: Partial<User>): Promise<User> {
  const dto: UserUpdateRequestDto = toUserUpdateRequestDto(user);
  const response = await api.put<UserResponseDto>("/me", dto);
  return toUser(response.data);
}

// DELETE /me
export async function deleteCurrentUser(): Promise<User> {
  const response = await api.delete<UserResponseDto>("/me");
  return toUser(response.data);
}