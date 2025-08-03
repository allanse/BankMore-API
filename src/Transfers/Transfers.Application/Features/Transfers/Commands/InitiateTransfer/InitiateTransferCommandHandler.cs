using MediatR;
using Transfers.Application.Contracts;
using Transfers.Domain;
using Transfers.Application.Exceptions;

namespace Transfers.Application.Features.Transfers.Commands.InitiateTransfer;

public class InitiateTransferCommandHandler : IRequestHandler<InitiateTransferCommand, Unit>
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITransferRepository _transferRepository;    

    public InitiateTransferCommandHandler(
        IAccountService accountService,
        ICurrentUserService currentUserService,
        ITransferRepository transferRepository)
    {
        _accountService = accountService;
        _currentUserService = currentUserService;
        _transferRepository = transferRepository;        
    }

    public async Task<Unit> Handle(InitiateTransferCommand request, CancellationToken cancellationToken)
    {       
        if (request.Valor <= 0)
        {
            throw new InvalidValueException("O valor da transferência deve ser positivo.");
        }

        var sourceAccountId = _currentUserService.GetCurrentUserId();
        var sourceAccount = await _accountService.GetAccountDetailsByIdAsync(sourceAccountId);

        if (sourceAccount is null)
        {
            throw new InvalidAccountException("Sua conta de origem é inválida ou não foi encontrada.");
        }
        if (!sourceAccount.Ativo)
        {
            throw new InactiveAccountException("Sua conta está inativa e não pode realizar transferências.");
        }

        var destinationAccount = await _accountService.GetAccountDetailsByNumberAsync(request.NumeroContaDestino);
        if (destinationAccount is null)
        {
            throw new InvalidOperationException("Conta de destino inválida ou não encontrada.");
        }

        if (!destinationAccount.Ativo)
        {
            throw new InvalidOperationException("Conta de destino está inativa.");
        }

        var idUsuarioLogado = _currentUserService.GetCurrentUserId();

        var debitRequest = new MovementRequest(null, request.Valor, 'D');
        try
        {
            await _accountService.PerformMovementAsync(debitRequest);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Falha ao debitar da conta de origem: {ex.Message}", ex);
        }

        var creditRequest = new MovementRequest(request.NumeroContaDestino, request.Valor, 'C');
        try
        {
            await _accountService.PerformMovementAsync(creditRequest);
        }
        catch (Exception ex)
        {
            var rollbackRequest = new MovementRequest(null, request.Valor, 'C');
            await _accountService.PerformMovementAsync(rollbackRequest);
            throw new InvalidOperationException($"Falha ao creditar na conta de destino, estorno realizado: {ex.Message}", ex);
        }
                
        var transferencia = Transferencia.Create(idUsuarioLogado, destinationAccount.IdContaCorrente, request.Valor);
        await _transferRepository.AddAsync(transferencia);        

        return Unit.Value;
    }
}