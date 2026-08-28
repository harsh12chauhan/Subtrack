namespace RenewalWorker
{
    public class Worker: BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public Worker(
            ILogger<Worker> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Renewal Worker Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation(
                    "Worker Heartbeat: {time}",
                    DateTime.UtcNow
                );

                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken);

                var client = httpClientFactory.CreateClient();

                logger.LogInformation("HttpClient Created Successfully");

            }
        }
    }
}
