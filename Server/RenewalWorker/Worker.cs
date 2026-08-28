using Microsoft.Extensions.Options;
using RenewalWorker.Configuration;
using RenewalWorker.Dto;

namespace RenewalWorker
{
    public class Worker: BackgroundService
    {
        private readonly ApiEndpoints apiEndpoints;
        private readonly ILogger<Worker> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public Worker(
            ILogger<Worker> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<ApiEndpoints> options
            )
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.apiEndpoints = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {            

            while (!stoppingToken.IsCancellationRequested)
            {

                await GetDueSubscriptions();  
                
                await Task.Delay(
                    TimeSpan.FromSeconds(15),
                    stoppingToken
                );

            }
        }

        private async Task GetDueSubscriptions() {

            var client = httpClientFactory.CreateClient();

            var response = await client.GetAsync($"{apiEndpoints.SubscriptionApi}/subscription/due");

            response.EnsureSuccessStatusCode();

            var subscriptions = await response.Content.ReadFromJsonAsync<List<DueSubscriptionDto>>();

            logger.LogInformation("Found {Count} due subscriptions", subscriptions?.Count ?? 0);
   
            foreach (var subscription in subscriptions ?? [])
            {
                logger.LogInformation(
                    "Subscription: {name} Amount: {amount}",
                    subscription.Name,
                    subscription.Amount
                );

                await ProcessPayment(subscription);
            }
        }

        private async Task ProcessPayment(DueSubscriptionDto dueSubscriptionDto) {

            try
            {
                var client = httpClientFactory.CreateClient();

                var paymentRequest = new CreatePaymentDto
                {
                    UserId = dueSubscriptionDto.UserId,
                    SubscriptionId = dueSubscriptionDto.Id,
                    Amount = dueSubscriptionDto.Amount
                };

                var response = await client.PostAsJsonAsync($"{apiEndpoints.PaymentApi}/payment/process", paymentRequest);

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Payment created for {subscription}", dueSubscriptionDto.Name);
                }
                else
                {
                    logger.LogError("Payment failed for {subscription}", dueSubscriptionDto.Name);
                }
            }
            catch (Exception ex){
                logger.LogError(ex,"Error processing payment for {subscription}", dueSubscriptionDto.Name);
            }
        }
    }
}
