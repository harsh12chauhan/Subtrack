namespace Payments.Dto
{
    public class ProcessPaymentDto
    {
        public Guid UserId { get; set; }

        public required Guid SubscriptionId { get; set; }

        public required decimal amount { get; set; }

    }
}
