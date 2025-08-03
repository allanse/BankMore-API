using System.Text.Json;
using Account.Application.Contracts;
using Account.Infrastructure.Data;
using Dapper;

namespace Account.Infrastructure.Services;

public class IdempotencyService : IIdempotencyService
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public IdempotencyService(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<string?> GetSavedResponseAsync(Guid requestId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @RequestId";

        return await connection.QuerySingleOrDefaultAsync<string>(sql, new { RequestId = requestId.ToString() });
    }
    
    public async Task StoreResponseAsync(Guid requestId, object request, int httpStatusCode, object response)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        var resultWrapper = new { StatusCode = httpStatusCode, Body = response };

        var requestJson = JsonSerializer.Serialize(request);
        var resultJson = JsonSerializer.Serialize(resultWrapper);

        const string sql = """
            INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado, criado_em)
            VALUES (@RequestId, @RequestJson, @ResultJson, @CreatedAt)
            """;

        await connection.ExecuteAsync(sql, new
        {
            RequestId = requestId.ToString(),
            RequestJson = requestJson,
            ResultJson = resultJson,
            CreatedAt = DateTime.UtcNow
        });
    }
}