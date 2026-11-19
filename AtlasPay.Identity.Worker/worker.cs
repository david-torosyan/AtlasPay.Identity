using AtlasPay.Identity.Application.Abstractions.Persistence;
using AtlasPay.Identity.Worker.Kafka;
using System.Text.Json;

namespace AtlasPay.Identity.Worker;

public sealed class Worker(
    IServiceScopeFactory scopeFactory,
    KafkaProducer kafkaProducer,
    IConfiguration configuration,
    ILogger<Worker> logger) : BackgroundService
{
    private readonly string _workerId =
        $"{Environment.MachineName}-{Guid.NewGuid():N}";

    private const int BatchSize = 20;

    private static readonly TimeSpan LeaseDuration =
        TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Identity outbox processing failed.");
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(2),
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var repository = scope.ServiceProvider
            .GetRequiredService<IOutboxPublisherRepository>();

        var messages = await repository.ClaimBatchAsync(
            _workerId,
            BatchSize,
            LeaseDuration,
            ct);

        foreach (var message in messages)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                using var document = JsonDocument.Parse(
                    message.Payload);

                var customerId = document.RootElement
                    .GetProperty("customerId")
                    .GetGuid();

                await kafkaProducer.PublishAsync(
                    configuration["Kafka:Topic"]!,
                    customerId.ToString("D"),
                    message.Payload,
                    ct);

                var marked = await repository.MarkProcessedAsync(
                    message.Id,
                    message.LockToken,
                    ct);

                if (!marked)
                {
                    logger.LogWarning(
                        "Outbox lease lost for message {MessageId}",
                        message.Id);
                }
            }
            catch (OperationCanceledException)
                when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                var delay = CalculateRetryDelay(
                    message.RetryCount);

                var scheduled = await repository.ScheduleRetryAsync(
                    message.Id,
                    message.LockToken,
                    DateTime.UtcNow.Add(delay),
                    ex.ToString(),
                    ct);

                if (!scheduled)
                {
                    logger.LogWarning(
                        "Cannot schedule retry: outbox lease lost for {MessageId}",
                        message.Id);
                }

                logger.LogError(
                    ex,
                    "Failed to publish outbox message {MessageId}",
                    message.Id);
            }
        }
    }

    private static TimeSpan CalculateRetryDelay(int retryCount)
    {
        var exponentialSeconds = Math.Min(
            300,
            2 * Math.Pow(2, Math.Min(retryCount, 8)));

        var jitterSeconds = Random.Shared.NextDouble() * 5;

        return TimeSpan.FromSeconds(
            exponentialSeconds + jitterSeconds);
    }
}