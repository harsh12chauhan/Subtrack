import { paymentApiClient } from "./paymentApiClient";
import type { createpayment } from "../../Types/payment";

export const paymentService = {
    
    createPayment: async (data: createpayment) => {
        const response = await paymentApiClient.post("/payment/process", data);

        return response.data;
    },

    paymentsBySubscriptionId: async (subscriptionId:string) => {
        const response = await paymentApiClient.get(`/payment/subscription/${subscriptionId}`);
        
        return response.data;
    },

    paymentByPaymentId: async (paymentId:string) => {
        const response = await paymentApiClient.get(`/payment/${paymentId}`);
        
        return response.data;
    },

    getAllUserPaymentTransactions: async () => {
        const response = await paymentApiClient.get("/payment/transactions");
        
        return response.data;
    },

    getAllPaymentTransactions: async () => {
        const response = await paymentApiClient.get("/payment/all");
        
        return response.data;
    },
    
};