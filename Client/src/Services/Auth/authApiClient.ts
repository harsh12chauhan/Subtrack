import axios from "axios";

export const authApiClient = axios.create({
  baseURL: "https://localhost:7025/",
});

authApiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});