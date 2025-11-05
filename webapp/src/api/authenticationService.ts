/*
  /webapp/src/api/authenticationService.ts

  This file contains the service functions to interact with the Authentication related API endpoints.
*/
import api from "./axios"
import type { UserCreateRequestDto, UserResponseDto} from "@/types/dtos/user"
import type { User } from "@/models/user";
import { toUser } from "@/mappers/userMapper";

// POST /authentication/login
export async function login(username: string, password: string): Promise<User> {
  const dto: UserCreateRequestDto = {
    username,
    password
  };
  const response = await api.post<UserResponseDto>("/authentication/login", dto);
  return toUser(response.data);
}

// POST /authentication/logout
export async function logout(): Promise<void> {
  await api.post("/authentication/logout");
}

// GET /authentication/me
export async function me(): Promise<User> {
  const response = await api.get<UserResponseDto>("/authentication/me");
  return toUser(response.data);
}