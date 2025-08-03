using Dapper;
using Transfers.Application.Contracts;
using Transfers.Domain;
using Transfers.Infrastructure.Data;

namespace Transfers.Infrastructure.Persistence;

public class TransferRepository : ITransferRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TransferRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task AddAsync(Transferencia transferencia)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO transferencia (idtransferencia, idcontacorrente_origem, idcontacorrente_destino, datamovimento, valor)
            VALUES (@IdTransferencia, @IdContaCorrenteOrigem, @IdContaCorrenteDestino, @DataMovimento, @Valor)
            """;

        await connection.ExecuteAsync(sql, transferencia);
    }
}