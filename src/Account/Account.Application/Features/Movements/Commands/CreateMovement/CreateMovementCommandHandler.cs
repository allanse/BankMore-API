using Account.Application.Contracts;
using Account.Application.Exceptions;
using Account.Domain;
using MediatR;

namespace Account.Application.Features.Movements.Commands.CreateMovement;

public class CreateMovementCommandHandler : IRequestHandler<CreateMovementCommand, Unit>
{
    private readonly ICurrentAccountRepository _repository;
    private readonly ICurrentUserService _currentUserService;    

    public CreateMovementCommandHandler(ICurrentAccountRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(CreateMovementCommand request, CancellationToken cancellationToken)
    {
        if (request.Valor <= 0)
        {
            throw new InvalidValueException("O valor da movimentação deve ser positivo.");
        }

        if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
        {
            throw new InvalidMovementTypeException("O tipo de movimentação é inválido. Utilize 'C' para Crédito ou 'D' para Débito.");
        }

        var loggedInUserAccount = await _repository.GetByIdAsync(_currentUserService.GetCurrentUserId());

        if (loggedInUserAccount is null || !loggedInUserAccount.Ativo)
        {
            throw new UnauthorizedAccessException("O usuário logado não possui uma conta válida para realizar a operação.");
        }

        var targetAccount = loggedInUserAccount;

        if (request.NumeroConta.HasValue && request.NumeroConta.Value != loggedInUserAccount.Numero)
        {
            targetAccount = await _repository.GetByAccountNumberAsync(request.NumeroConta.Value);
        }

        if (targetAccount is null)
        {
            throw new InvalidAccountException("A conta corrente informada para a movimentação não existe.");
        }

        if (!targetAccount.Ativo)
        {
            throw new InactiveAccountException("A conta corrente informada para a movimentação está inativa.");
        }

        if (targetAccount.IdContaCorrente != loggedInUserAccount.IdContaCorrente && request.TipoMovimento == 'D')
        {
            throw new InvalidMovementTypeException("Não é permitido realizar um débito em contas de terceiros.");
        }

        if (request.TipoMovimento == 'D')
        {
            var currentBalance = await _repository.GetCurrentBalanceAsync(targetAccount.IdContaCorrente);
            if (currentBalance < request.Valor)
            {
                throw new InsufficientFundsException("Saldo insuficiente para realizar a operação.");
            }
        }

        var movimento = Movimento.Create(targetAccount.IdContaCorrente, request.TipoMovimento, request.Valor);

        await _repository.AddMovementAsync(movimento);

        return Unit.Value;
    }
}