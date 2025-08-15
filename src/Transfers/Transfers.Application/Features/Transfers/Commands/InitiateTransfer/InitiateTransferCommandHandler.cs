using BankMore.Common.Messaging.Events;
using MediatR;
using Transfers.Application.Contracts;
using Transfers.Application.Exceptions;

namespace Transfers.Application.Features.Transfers.Commands.InitiateTransfer;

public class InitiateTransferCommandHandler : IRequestHandler<InitiateTransferCommand, Unit>
{
    private readonly IMessagePublisher _producer;
    private readonly ICurrentUserService _currentUserService;     

    public InitiateTransferCommandHandler(
        IMessagePublisher producer,
        ICurrentUserService currentUserService)
    {        
        _currentUserService = currentUserService;
        _producer = producer;
    }

    public async Task<Unit> Handle(InitiateTransferCommand request, CancellationToken cancellationToken)
    {       
        if (request.Valor <= 0)
        {
            throw new InvalidValueException("O valor da transferência deve ser positivo.");
        }       

        var sourceAccountId = _currentUserService.GetCurrentUserId();

        var transferEvent = new TransferenciaIniciadaEvent
        {
            IdRequisicao = request.IdRequisicao,
            IdContaOrigem = sourceAccountId,
            NumeroContaDestino = request.NumeroContaDestino,
            Valor = request.Valor
        };
        
        await _producer.ProduceAsync(transferEvent);

        return Unit.Value;
    }
}