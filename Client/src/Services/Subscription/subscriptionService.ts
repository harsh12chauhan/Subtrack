import { subscriptionApiClient } from "./subscriptionApiClient";
import type { subscription } from "../../Types/subscription"

export const subscriptionService = {
    
    create: async (data: subscription) => {
        const response = await subscriptionApiClient.post("/subscription/create", data);

        return response.data;
    },

    updateSubscription: async (data: subscription, subscriptionId:string) => {
        const response = await subscriptionApiClient.patch(`/subscription/update/${subscriptionId}`, data);
        
        return response.data;
    },
    
    pauseSubscription: async (subscriptionId:string) => {
        const response = await subscriptionApiClient.put(`/subscription/status/${subscriptionId}/Paused`);

        return response.data;
    },

    activeSubscription: async (subscriptionId:string) => {
        const response = await subscriptionApiClient.put(`/subscription/status/${subscriptionId}/Active`);

        return response.data;
    },

    cancelSubscription: async (subscriptionId:string) => {
        const response = await subscriptionApiClient.put(`/subscription/status/${subscriptionId}/Cancelled`);
        
        return response.data;
    },
    
    userSubscriptions: async () => {
        const response = await subscriptionApiClient.get("/subscription/user-subscription");
        
        return response.data;
    },
    
    all: async () => {
        const response = await subscriptionApiClient.get("/subscription/all");

        return response.data;
    },
    
    getSubscriptions: async (subscriptionId: string) => {
        const response = await subscriptionApiClient.get(`/subscription/${subscriptionId}`);
        
        return response.data;
    },

    deleteSubscriptions: async (subscriptionId: string) => {
        const response = await subscriptionApiClient.delete(`/subscription/${subscriptionId}`);

        return response.data;
    },
};