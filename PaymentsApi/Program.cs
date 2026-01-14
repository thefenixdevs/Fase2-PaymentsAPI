using MassTransit;
using PaymentsApi;
using PaymentsApi.Consumers;
using PaymentsApi.Infrastructure.Configuration;
using Shared.Contracts.Events;

var builder = Host.CreateApplicationBuilder(args);

var queueSettings = QueueSettingsFactory.FromEnvironment();

builder.Services.AddMassTransit(x =>
{
  x.AddConsumer<OrderPlacedConsumer>();

  x.UsingRabbitMq((context, cfg) =>
  {
      cfg.Host(
        builder.Configuration["RabbitMQ:Host"],
        builder.Configuration["RabbitMQ:VirtualHost"],
        h =>
        {
          h.Username(builder.Configuration["RabbitMQ:Username"]);
          h.Password(builder.Configuration["RabbitMQ:Password"]);
        });

    cfg.ReceiveEndpoint(queueSettings.OrderPlacedEventQueue, e =>
    {
      e.ConfigureConsumer<OrderPlacedConsumer>(context);
    });

    cfg.Message<PaymentProcessedEvent>(x =>
    {
        x.SetEntityName("Shared.Contracts.Events:PaymentProcessedEvent");
    });
  });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
