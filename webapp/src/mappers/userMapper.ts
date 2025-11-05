/*
  /webapp/src/mappers/userMapper.ts

  This file contains the mapping functions to convert between User DTOs and User models.
*/
import type { User } from "@/models/user";
import type {
  UserResponseDto,
  UserCreateRequestDto,
  UserUpdateRequestDto,
} from "@/types/dtos/user";

// DTOs -> Models:
// maps a UserResponseDto to a User model
export function toUser(dto: UserResponseDto): User {
  return {
    id: dto.id,
    username: dto.username,
    posts: dto.postIds ?? [],
    comments: dto.commentIds ?? [],
    isAdmin: dto.isAdmin,
    isBanned: dto.isBanned,
    createdAt: new Date(dto.createdAt),
    updatedAt: new Date(dto.updatedAt),
  };
}

// maps an array of UserResponseDto to an array of User models
export function toUsers(dtos: UserResponseDto[]): User[] {
  return dtos.map(toUser);
}

// Models -> DTOs:
export function toUserCreateRequestDto(username: string, password: string): UserCreateRequestDto {
  return {
    username,
    password,
  };
}

export function toUserUpdateRequestDto(user: Partial<User>): UserUpdateRequestDto {
  const dto: UserUpdateRequestDto = {};
  if (user.username !== undefined) {
    dto.username = user.username;
  }

  return dto;
}
