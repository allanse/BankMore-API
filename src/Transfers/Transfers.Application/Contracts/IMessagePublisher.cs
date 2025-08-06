namespace Transfers.Application.Contracts;

public interface IMessagePublisher
{
    Task ProduceAsync<T>(T message);
}