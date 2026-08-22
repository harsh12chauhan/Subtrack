import { authApiClient } from "./authApiClient";

import type { LoginRequest, RegisterRequest } from "../../Types/auth";

export const authService = {
  register: async (data: RegisterRequest) => {
    const response = await authApiClient.post("/auth/register", data);

    return response.data;
  },

  login: async (data: LoginRequest) => {
    const response = await authApiClient.post("/auth/login", data);

    return response.data;
  },
};