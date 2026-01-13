using MassTransit;
using Shared.Contracts.Events;

namespace PaymentsApi.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
  public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
  {
    var message = context.Message;

    Console.WriteLine($"[PaymentsAPI] Processando pagamento do pedido {message.OrderId}");

    var approved = message.Price > 100;

    var paymentResult = new PaymentProcessedEvent(
        message.OrderId,
        message.UserId,
        message.GameId,
        approved ? "Aprovado" : "Rejeitado"
    );

    await context.Publish(paymentResult);

    Console.WriteLine($"[PaymentsAPI] Pagamento {paymentResult.Status} para pedido {message.OrderId}");
  }
}
