using Confluent.Kafka;

namespace AtlasPay.Identity.Worker.Kafka;

public sealed class KafkaProducer(
    IProducer<string, string> producer)
{
    public async Task PublishAsync(
        string topic,
        string key,
        string payload,
        CancellationToken ct)
    {
        var result = await producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = key,
                Value = payload
            },
            ct);

        if (result.Status != PersistenceStatus.Persisted)
        {
            throw new KafkaException(
                new Error(
                    ErrorCode.Local_Fail,
                    "Kafka did not confirm persistence."));
        }
    }
}