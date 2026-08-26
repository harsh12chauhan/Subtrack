namespace Payments.Dto
{
    public class ProcessPaymentDto
    {
        public required Guid SubscriptionId { get; set; }

        public required decimal SubscriptionAmount { get; set; }

    }
}
