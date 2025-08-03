using Account.Application.Contracts;
using Account.Application.Features.Accounts.Queries.GetAccountBalance;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Account.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery, IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IDistributedCache cache, ICurrentUserService currentUserService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {        
        if (request is GetAccountBalanceQuery balanceQuery)
        {
            balanceQuery.AccountId = _currentUserService.GetCurrentUserId();
        }

        var cachedResponse = await _cache.GetStringAsync(request.CacheKey, cancellationToken);

        if (cachedResponse is not null)
        {
            _logger.LogInformation("Cache HIT para a chave {CacheKey}", request.CacheKey);
            return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
        }

        _logger.LogInformation("Cache MISS para a chave {CacheKey}", request.CacheKey);

        var response = await next();
        
        var cacheOptions = new DistributedCacheEntryOptions
        {            
            AbsoluteExpirationRelativeToNow = request.Expiration
        };

        var serializedResponse = JsonSerializer.Serialize(response);
        
        await _cache.SetStringAsync(request.CacheKey, serializedResponse, cacheOptions, cancellationToken);

        return response;
    }
}