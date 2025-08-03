using MediatR;
namespace Account.Application.Features.Accounts.Queries.GetAccountByNumber;

public class GetAccountByNumberQuery : IRequest<AccountDetailsResponse?>
{
    public int AccountNumber { get; set; }
}