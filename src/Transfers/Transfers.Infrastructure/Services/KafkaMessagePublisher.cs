using KafkaFlow.Producers;
using Transfers.Application.Contracts;

namespace Transfers.Infrastructure.Services;

public class KafkaMessagePublisher : IMessagePublisher
{
    private readonly IProducerAccessor _producerAccessor;

    public KafkaMessagePublisher(IProducerAccessor producerAccessor)
    {
        _producerAccessor = producerAccessor;
    }

    public Task ProduceAsync<T>(T message)
    {        
        var producer = _producerAccessor.GetProducer("transfer-producer");

        return producer.ProduceAsync(           
            Guid.NewGuid().ToString(),
            message
        );
    }
}