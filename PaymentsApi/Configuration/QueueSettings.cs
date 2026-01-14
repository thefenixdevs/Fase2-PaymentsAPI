namespace PaymentsApi.Infrastructure.Configuration
{
    public class QueueSettings
    {
        public string OrderPlacedEventQueue { get; init; } = default!;
    }
}
