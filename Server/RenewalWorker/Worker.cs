using Microsoft.Extensions.Options;
using RenewalWorker.Configuration;
using RenewalWorker.Dto;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace RenewalWorker
{
    public class Worker: BackgroundService
    {
        private readonly WorkerCredientials workerCredientials;
        private readonly ApiEndpoints apiEndpoints;
        private readonly ILogger<Worker> logger;
        private readonly IHttpClientFactory httpClientFactory;

        private string? JwtToken;        

        public Worker(
            ILogger<Worker> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<WorkerCredientials> workerCredientialsOptions,
            IOptions<ApiEndpoints> apiEndpointsOptions
            )
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.apiEndpoints = apiEndpointsOptions.Value;
            this.workerCredientials = workerCredientialsOptions.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {            

            while (!stoppingToken.IsCancellationRequested)
            {                
                await GetJwtToken();

                logger.LogInformation(
    "Token Retrieved: {token}",
    JwtToken?.Substring(0, 20)
);

                await GetDueSubscriptions();  
                
                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken
                );

            }
        }

        private async Task GetDueSubscriptions() {

            try
            {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

                var response = await client.GetAsync($"{apiEndpoints.SubscriptionApi}/subscription/due");
                response.EnsureSuccessStatusCode();

                var subscriptions = await response.Content.ReadFromJsonAsync<List<DueSubscriptionDto>>();

                logger.LogInformation("Found {Count} due subscriptions", subscriptions?.Count ?? 0);

                foreach (var subscription in subscriptions ?? [])
                {
                    logger.LogInformation("Subscription: {name} Amount: {amount}", subscription.Name, subscription.Amount);

                    try
                    {
                        var paymentSuccess = await ProcessPayment(subscription);                    

                        await CreateNotification(subscription, paymentSuccess);

                        if (paymentSuccess)
                        {
                            await RenewSubscription(subscription);
                            logger.LogInformation("Subscription {subscription} for user {userId} renewed successfully", subscription.Name, subscription.UserId);
                        }

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed processing subscription {id}", subscription.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Internal Server Error Occured at worker");
            }
        }

        private async Task<bool> ProcessPayment(DueSubscriptionDto subscription) {

            try
            {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

                var paymentRequest = new CreatePaymentDto
                {
                    UserId = subscription.UserId,
                    SubscriptionId = subscription.Id,
                    Amount = subscription.Amount
                };

                var response = await client.PostAsJsonAsync($"{apiEndpoints.PaymentApi}/payment/process", paymentRequest);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Payment created for {subscription}", subscription.Name);
                    return true;
                }
                else
                {
                    logger.LogError("Payment failed for {subscription}", subscription.Name);
                    return false;
                }
            }
            catch (Exception ex){
                logger.LogError(ex,"Error processing payment for {subscription}", subscription.Name);
                return false;
            }
        }

        private async Task CreateNotification(DueSubscriptionDto subscription, bool paymentSucceeded) {

            try {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

                var notification = new CreateNotificationDto{
                        
                    UserId = subscription.UserId,
                    Title = paymentSucceeded ? "Payment Successful": "Payment Failed",
                    Message = paymentSucceeded? $"{subscription.Name} renewed successfully.": $"{subscription.Name} payment failed.",
                    Type = paymentSucceeded ? 1 : 2
                };

                var response = await client.PostAsJsonAsync($"{apiEndpoints.NotificationApi}/notification/create",notification);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation(
                    "Notification created for {subscription}",
                    subscription.Name);
                }
                else
                {
                    logger.LogError(
                    "Notification creation failed for {subscription}",
                    subscription.Name);
                }


            }
            catch (Exception ex) {
                logger.LogError(ex,"Notification error for {subscription}",subscription.Name);
            }

        }

        private async Task RenewSubscription(DueSubscriptionDto subscription)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);

                var response = await client.PatchAsync($"{apiEndpoints.SubscriptionApi}/subscription/renew/{subscription.Id}",null);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation(
                        "Subscription renewed: {name}",
                        subscription.Name);
                }
                else
                {
                    logger.LogError(
                        "Failed to renew subscription: {name}",
                        subscription.Name);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Renewal error for {name}",
                    subscription.Name);
            }
        }

        private async Task GetJwtToken() {

            try
            {
                var client = httpClientFactory.CreateClient();

                var loginRequest = new LoginDto
                {
                    Email = workerCredientials.Email,
                    Password = workerCredientials.Password
                };

                var response = await client.PostAsJsonAsync($"{apiEndpoints.AuthApi}/auth/login", loginRequest);

                response.EnsureSuccessStatusCode();
                
                var authResponseDto = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                JwtToken = authResponseDto?.Token;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate Jwt Token");
            }
        }
    }
}
