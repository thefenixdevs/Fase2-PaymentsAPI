using MassTransit;
using PaymentsApi;
using PaymentsApi.Consumers;
using Microsoft.Extensions.Hosting;
using Shared.Contracts.Events;

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

    // Configure JSON serializer to use the same options and respect JsonConverter attributes
    cfg.ConfigureJsonSerializerOptions(options =>
    {
      options.PropertyNamingPolicy = null; // PascalCase
      // Ensure converters are used
      options.Converters.Add(new DecimalStringConverter());
      return options;
    });

    // Bind to existing exchange created by CatalogAPI (producer)
    // IMPORTANT: Do not let MassTransit create the exchange - only bind to existing one
    // The exchange "fcg.order-placed-event" is a FANOUT exchange, so no routing key is needed
    cfg.ReceiveEndpoint("fcg.payments.order-placed", e =>
    {
      // Do not let this endpoint generate topology automatically (prevents exchange creation)
      e.ConfigureConsumeTopology = false;

      // Bind to existing fanout exchange without creating it
      // Fanout exchanges ignore routing keys, so we don't specify one
      e.Bind("fcg.order-placed-event");
      
      // Configure retry policy for transient errors (3 retries with 5 second intervals)
      e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
      
      e.ConfigureConsumer<OrderPlacedConsumer>(context);
    });

    // Configure explicit entity name for PaymentProcessedEvent
    cfg.Message<PaymentProcessedEvent>(m =>
    {
      m.SetEntityName("fcg.payment-processed-event");
    });
  });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
