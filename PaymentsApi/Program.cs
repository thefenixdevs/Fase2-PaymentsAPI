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

    cfg.ReceiveEndpoint("order-placed-queue", e =>
    {
      e.ConfigureConsumer<OrderPlacedConsumer>(context);
    });
  });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
