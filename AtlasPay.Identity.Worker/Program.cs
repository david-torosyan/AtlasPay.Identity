using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Infrastructure.Persistence;
using AtlasPay.Identity.Infrastructure.Persistence.Repositories;
using AtlasPay.Identity.Worker;
using AtlasPay.Identity.Worker.Kafka;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("IdentityDb")));

builder.Services.AddScoped<
    IOutboxPublisherRepository,
    OutboxPublisherRepository>();

builder.Services.AddSingleton<IProducer<string, string>>(_ =>
    new ProducerBuilder<string, string>(new ProducerConfig
    {
        BootstrapServers =
            builder.Configuration["Kafka:BootstrapServers"],
        Acks = Acks.All,
        EnableIdempotence = true
    }).Build());

builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();