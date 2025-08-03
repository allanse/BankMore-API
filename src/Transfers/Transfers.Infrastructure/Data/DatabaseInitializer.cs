using Dapper;
namespace Transfers.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    public DatabaseInitializer(IDbConnectionFactory dbConnectionFactory) { _dbConnectionFactory = dbConnectionFactory; }
    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string transferenciaSql = """
            CREATE TABLE IF NOT EXISTS transferencia (
                idtransferencia TEXT PRIMARY KEY,
                idcontacorrente_origem TEXT NOT NULL,
                idcontacorrente_destino TEXT NOT NULL,
                datamovimento TEXT NOT NULL,
                valor REAL NOT NULL
            );
            """;
        await connection.ExecuteAsync(transferenciaSql);

        const string idempotenciaSql = """
            CREATE TABLE IF NOT EXISTS idempotencia (
                chave_idempotencia TEXT PRIMARY KEY,
                requisicao TEXT NOT NULL,
                resultado TEXT NOT NULL,
                criado_em TEXT NOT NULL
            );
            """;
        await connection.ExecuteAsync(idempotenciaSql);
    }

    
}