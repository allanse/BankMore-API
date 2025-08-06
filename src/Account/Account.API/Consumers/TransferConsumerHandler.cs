using Account.Application.Contracts;
using Account.Domain;
using KafkaFlow;

namespace Account.API.Consumers;

public class TransferenciaIniciadaEvent
{
    public Guid IdRequisicao { get; set; }
    public Guid IdContaOrigem { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}

public class TransferConsumerHandler : IMessageHandler<TransferenciaIniciadaEvent>
{
    private readonly ICurrentAccountRepository _repository;
    private readonly ILogger<TransferConsumerHandler> _logger;

    public TransferConsumerHandler(ICurrentAccountRepository repository, ILogger<TransferConsumerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, TransferenciaIniciadaEvent message)
    {
        _logger.LogInformation(
            "Consumindo evento de transferência. Chave: {Key}, Origem: {Source}, Destino: {Destination}, Valor: {Value}",
            context.Message.Key, message.IdContaOrigem, message.NumeroContaDestino, message.Valor);

        var destinationAccount = await _repository.GetByAccountNumberAsync(message.NumeroContaDestino);
        if (destinationAccount is null || !destinationAccount.Ativo)
        {
            _logger.LogError("Transferência {TransferId} falhou: Conta de destino inválida ou inativa.", message.IdRequisicao);
            
            return;
        }

        var sourceBalance = await _repository.GetCurrentBalanceAsync(message.IdContaOrigem);
        if (sourceBalance < message.Valor)
        {
            _logger.LogError("Transferência {TransferId} falhou: Saldo insuficiente na conta de origem.", message.IdRequisicao);
            return;
        }

        var debitMovement = Movimento.Create(message.IdContaOrigem, 'D', message.Valor);
        var creditMovement = Movimento.Create(destinationAccount.IdContaCorrente, 'C', message.Valor);
        
        await _repository.AddMovementAsync(debitMovement);
        await _repository.AddMovementAsync(creditMovement);

        _logger.LogInformation("Transferência {TransferId} processada com sucesso!", message.IdRequisicao);
    }
}