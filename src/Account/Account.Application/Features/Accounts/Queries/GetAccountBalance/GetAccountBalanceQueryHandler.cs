using Account.Application.Contracts;
using Account.Application.Exceptions;
using MediatR;

namespace Account.Application.Features.Accounts.Queries.GetAccountBalance;

public class GetAccountBalanceQueryHandler : IRequestHandler<GetAccountBalanceQuery, GetAccountBalanceQueryResponse>
{
    private readonly ICurrentAccountRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetAccountBalanceQueryHandler(ICurrentAccountRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<GetAccountBalanceQueryResponse> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
    {        
        var accountId = _currentUserService.GetCurrentUserId();
        
        var account = await _repository.GetByIdAsync(accountId);
        
        if (account is null)
        {
            throw new InvalidAccountException("A conta do usuário logado não foi encontrada.");
        }
        
        if (!account.Ativo)
        {
            throw new InactiveAccountException("A sua conta corrente está inativa e não pode consultar o saldo.");
        }
        
        var balance = await _repository.GetCurrentBalanceAsync(accountId);
        
        return new GetAccountBalanceQueryResponse
        {
            NumeroConta = account.Numero,
            NomeTitular = account.Nome,
            DataConsulta = DateTime.UtcNow,
            Saldo = balance
        };
    }
}