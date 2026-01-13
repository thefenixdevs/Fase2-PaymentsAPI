using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Contracts.Events;

var services = new ServiceCollection();

var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
services.AddMassTransit(x =>
{
  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.Host("localhost", "/", h =>
    {
      h.Username("guest");
      h.Password("guest");
    });
  });
});

var provider = services.BuildServiceProvider();
var bus = provider.GetRequiredService<IBusControl>();

await bus.StartAsync();

await bus.Publish(new OrderPlacedEvent(
    Guid.NewGuid(),
    Guid.NewGuid(),
    Guid.NewGuid(),
    50
));

await bus.Publish(new OrderPlacedEvent(
    Guid.NewGuid(),
    Guid.NewGuid(),
    Guid.NewGuid(),
    0
));

await bus.Publish(new OrderPlacedEvent(
    Guid.NewGuid(),
    Guid.NewGuid(),
    Guid.NewGuid(),
    1000
));

await bus.Publish(new OrderPlacedEvent(
    Guid.NewGuid(),
    Guid.NewGuid(),
    Guid.NewGuid(),
    101
));

Console.WriteLine("OrderPlacedEvent publicado!");

await bus.StopAsync();
