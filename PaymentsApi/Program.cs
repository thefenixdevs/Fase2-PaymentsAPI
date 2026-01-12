using MassTransit;
using PaymentsApi;
using PaymentsApi.Consumers;

var builder = Host.CreateApplicationBuilder(args);

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

    // Bind to existing exchange created by CatalogAPI (producer)
    // IMPORTANT: Do not let MassTransit create the exchange - only bind to existing one
    cfg.ReceiveEndpoint("fcg.payments.order-placed", e =>
    {
      // Do not let this endpoint generate topology automatically (prevents exchange creation)
      e.ConfigureConsumeTopology = false;

      // Bind to existing exchange without creating it
      // The exchange "fcg.order-placed-event" must already exist (created by CatalogAPI)
      e.Bind("fcg.order-placed-event", s =>
      {
        // Only specify routing key, let producer (CatalogAPI) handle exchange creation
        s.RoutingKey = "payments.order-placed"; // Consume messages with routing key payments.order-placed
        // Do not set ExchangeType - this prevents MassTransit from trying to create it
        // MassTransit will only bind the queue to the existing exchange
      });
      
      e.ConfigureConsumer<OrderPlacedConsumer>(context);
    });

    // Configure explicit entity name for PaymentProcessedEvent
    cfg.Message<PaymentsApi.Contracts.Events.PaymentProcessedEvent>(m =>
    {
      m.SetEntityName("fcg.payment-processed-event");
    });
  });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
