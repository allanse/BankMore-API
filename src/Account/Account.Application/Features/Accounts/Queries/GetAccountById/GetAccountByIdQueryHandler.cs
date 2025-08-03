using MediatR;
using Account.Application.Contracts;
using Account.Application.Features.Accounts.Queries.GetAccountByNumber;

namespace Account.Application.Features.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDetailsResponse?>
{
    private readonly ICurrentAccountRepository _repository;

    public GetAccountByIdQueryHandler(ICurrentAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountDetailsResponse?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId);

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