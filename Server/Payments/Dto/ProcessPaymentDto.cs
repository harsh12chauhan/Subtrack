namespace Payments.Dto
{
    public class ProcessPaymentDto
    {
        public required Guid SubscriptionId { get; set; }

        public required decimal Amount { get; set; }

    }
}
