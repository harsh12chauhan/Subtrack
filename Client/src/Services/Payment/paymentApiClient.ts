import axios from "axios";

export const paymentApiClient = axios.create({
  baseURL: "https://localhost:7070/",
});

paymentApiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});