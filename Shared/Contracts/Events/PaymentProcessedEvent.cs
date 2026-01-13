namespace Shared.Contracts.Events
{
    public class PaymentProcessedEvent
    {
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
        public string Status { get; private set; } = string.Empty;
        protected PaymentProcessedEvent() { }

        public PaymentProcessedEvent(
            Guid orderId,
            Guid userId,
            Guid gameId,
            string status)
        {
            OrderId = orderId;
            UserId = userId;
            GameId = gameId;
            Status = status;
        }
    }
}
