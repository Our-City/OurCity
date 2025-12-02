/*
  /webapp/src/api/authorizationService.ts

  This file contains the service functions to interact with the Authorization related API endpoints.
*/
import api from "./axios";

// GET /authorization/can-participate-in-forum
export async function canParticipateInForum(): Promise<boolean> {
  const response = await api.get<boolean>("/authorization/can-participate-in-forum");
  return response.data;
}

// GET /authorization/can-view-admin-dashboard
export async function canViewAdminDashboard(): Promise<boolean> {
  const response = await api.get<boolean>("/authorization/can-view-admin-dashboard");
  return response.data;
}
