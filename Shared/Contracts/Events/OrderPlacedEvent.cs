namespace Shared.Contracts.Events;

public class OrderPlacedEvent
{
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public Decimal Price { get; private set; } = default;

    protected OrderPlacedEvent() { }

    public OrderPlacedEvent(Guid orderId, Guid userId, Guid gameId, Decimal price)
    {
        OrderId = orderId;
        UserId = userId;
        GameId = gameId;
        Price = price;
    }
}