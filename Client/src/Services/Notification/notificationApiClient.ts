import axios from "axios";

export const notificationApiClient = axios.create({
  baseURL: "https://localhost:7056/",
});

notificationApiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});