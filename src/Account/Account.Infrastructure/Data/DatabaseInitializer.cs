using Dapper;

namespace Account.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DatabaseInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = """
            CREATE TABLE IF NOT EXISTS contacorrente (
                idcontacorrente TEXT PRIMARY KEY,
                numero INTEGER NOT NULL UNIQUE,
                nome TEXT NOT NULL,
                cpf TEXT NOT NULL UNIQUE,
                senha TEXT NOT NULL,
                ativo INTEGER NOT NULL
            );
            """;
        await connection.ExecuteAsync(sql);

        const string movimentoSql = """
        CREATE TABLE IF NOT EXISTS movimento (
            idmovimento TEXT PRIMARY KEY,
            idcontacorrente TEXT NOT NULL,
            datamovimento TEXT NOT NULL,
            tipomovimento TEXT NOT NULL,
            valor REAL NOT NULL,
            FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
        );
        """;
        await connection.ExecuteAsync(movimentoSql);

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