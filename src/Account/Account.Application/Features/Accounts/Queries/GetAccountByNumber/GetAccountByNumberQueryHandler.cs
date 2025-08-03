using Account.Application.Contracts;
using MediatR;
namespace Account.Application.Features.Accounts.Queries.GetAccountByNumber;

public class GetAccountByNumberQueryHandler : IRequestHandler<GetAccountByNumberQuery, AccountDetailsResponse?>
{
    private readonly ICurrentAccountRepository _repository;

    public GetAccountByNumberQueryHandler(ICurrentAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountDetailsResponse?> Handle(GetAccountByNumberQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByAccountNumberAsync(request.AccountNumber);

        if (account is null)
        {
            return null;
        }

        return new AccountDetailsResponse
        {
            IdContaCorrente = account.IdContaCorrente,
            Ativo = account.Ativo
        };
    }
}