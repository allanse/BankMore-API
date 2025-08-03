using System.Text.Json;
using Transfers.Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Transfers.Application.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(IIdempotencyService idempotencyService, ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _idempotencyService = idempotencyService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var savedResultJson = await _idempotencyService.GetSavedResponseAsync(request.IdRequisicao);

        if (savedResultJson is not null)
        {
            _logger.LogInformation("Requisição duplicada detectada. Retornando resultado salvo para a chave {RequestId}", request.IdRequisicao);

            var savedResult = JsonSerializer.Deserialize<JsonElement>(savedResultJson);

            if (savedResult.GetProperty("StatusCode").GetInt32() >= 400)
            {
                var errorBody = savedResult.GetProperty("Body").GetString();
                throw new ApplicationException(errorBody);
            }

            var responseBody = savedResult.GetProperty("Body").Deserialize<TResponse>();
            return responseBody!;
        }

        var response = await next();

        await _idempotencyService.StoreResponseAsync(request.IdRequisicao, request, StatusCodes.Status204NoContent, new { status = "Success" });

        return response;
    }
}