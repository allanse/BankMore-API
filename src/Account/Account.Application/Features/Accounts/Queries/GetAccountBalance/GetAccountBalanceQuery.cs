using Account.Application.Contracts;
using MediatR;

namespace Account.Application.Features.Accounts.Queries.GetAccountBalance;

public class GetAccountBalanceQuery : IRequest<GetAccountBalanceQueryResponse>, ICacheableQuery
{    
    public Guid AccountId { get; set; }

    public string CacheKey => $"balance-{AccountId}";
    public TimeSpan Expiration => TimeSpan.FromSeconds(30);
}