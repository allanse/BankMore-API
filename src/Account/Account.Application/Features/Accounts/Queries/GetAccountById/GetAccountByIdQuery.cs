using MediatR;
using Account.Application.Features.Accounts.Queries.GetAccountByNumber;

namespace Account.Application.Features.Accounts.Queries.GetAccountById;

public class GetAccountByIdQuery : IRequest<AccountDetailsResponse?>
{
    public Guid AccountId { get; set; }
}