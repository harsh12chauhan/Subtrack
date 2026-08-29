import { notificationApiClient } from "./notificationApiClient";

// import type { Notification } from "../../Types/notification";

export const notificationService = {

    GetNotifications: async () => {
        const response = await notificationApiClient.get("notification/my");

        return response.data;
    },
    
    ReadAllNotifications: async () => {
        const response = await notificationApiClient.patch("notification/readall");
        
        return response.data;
    },

    ReadNotification: async (notificationId: string) => {
        const response = await notificationApiClient.patch(`notification/read/${notificationId}`);

        return response.data;
    },
    
    GetUnreadNotificationCount: async () => {
        const response = await notificationApiClient.get("notification/unreadcount");
        
        return response.data;
    },

    GetAllNotifications: async () => {
        const response = await notificationApiClient.get("notification/all");

        return response.data;
    },

    deleteSubscriptions: async (notificationId: string) => {
        const response = await notificationApiClient.delete(`/notification/${notificationId}`);

        return response.data;
    },

};