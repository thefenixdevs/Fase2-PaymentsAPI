namespace PaymentsApi.Infrastructure.Configuration
{
    public static class QueueSettingsFactory
    {
        public static QueueSettings FromEnvironment()
        {
            return new QueueSettings
            {
                OrderPlacedEventQueue =
                    Environment.GetEnvironmentVariable("QUEUE_ORDER_PLACED")
                    ?? "order-placed-event-queue"
            };
        }
    }
}
