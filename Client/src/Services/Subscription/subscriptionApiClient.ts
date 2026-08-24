import axios from "axios";

export const subscriptionApiClient = axios.create({
  baseURL: "https://localhost:7081/",
});
