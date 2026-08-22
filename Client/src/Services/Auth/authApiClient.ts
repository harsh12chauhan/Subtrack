import axios from "axios";

export const authApiClient = axios.create({
  baseURL: "https://localhost:7025/",
});
