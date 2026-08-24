import axios from "axios";

export const paymentApiClient = axios.create({
  baseURL: "https://localhost:7070/",
});
