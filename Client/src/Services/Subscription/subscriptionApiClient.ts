import axios from "axios";

export const subscriptionApiClient = axios.create({
  baseURL: "https://localhost:7081/",
});

subscriptionApiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});