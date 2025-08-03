using Account.Application.Contracts;
using Account.Domain;
using Account.Infrastructure.Data;
using Dapper;

namespace Account.Infrastructure.Persistence;

public class CurrentAccountRepository : ICurrentAccountRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public CurrentAccountRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CpfExistsAsync(string cpf)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = "SELECT COUNT(1) FROM ContaCorrente WHERE Cpf = @Cpf";
        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Cpf = cpf });
        return exists;
    }

    public async Task<int> GetNextAccountNumberAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();        
        const string sql = "SELECT COALESCE(MAX(Numero), 10000) + 1 FROM ContaCorrente";
        var nextNumber = await connection.ExecuteScalarAsync<int>(sql);
        return nextNumber;
    }

    public async Task CreateAsync(CurrentAccount conta)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = """
            INSERT INTO ContaCorrente (idcontacorrente, numero, nome, cpf, senha, ativo)
            VALUES (@IdContaCorrente, @Numero, @Nome, @Cpf, @Senha, @Ativo)
            """;

        await connection.ExecuteAsync(sql, conta);
    }

    public async Task<CurrentAccount?> GetByCpfOrAccountNumberAsync(string cpfOrAccountNumber)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        var isNumber = int.TryParse(cpfOrAccountNumber, out var accountNumber);

        var sql = isNumber
            ? "SELECT * FROM contacorrente WHERE numero = @AccountNumber"
            : "SELECT * FROM contacorrente WHERE cpf = @Cpf";

        var parameters = isNumber
            ? (object)new { AccountNumber = accountNumber }
            : new { Cpf = cpfOrAccountNumber };

        var account = await connection.QueryFirstOrDefaultAsync<CurrentAccount>(sql, parameters);

        return account;
    }

    public async Task<CurrentAccount?> GetByIdAsync(Guid accountId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM contacorrente WHERE idcontacorrente = @AccountId";
        return await connection.QueryFirstOrDefaultAsync<CurrentAccount>(sql, new { AccountId = accountId });
    }

    public async Task UpdateAsync(CurrentAccount account)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = """
        UPDATE contacorrente SET
            nome = @Nome,
            senha = @Senha,
            ativo = @Ativo
        WHERE idcontacorrente = @IdContaCorrente
        """;
        await connection.ExecuteAsync(sql, account);
    }

    public async Task<CurrentAccount?> GetByAccountNumberAsync(int accountNumber)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = "SELECT * FROM contacorrente WHERE numero = @AccountNumber";
        return await connection.QueryFirstOrDefaultAsync<CurrentAccount>(sql, new { AccountNumber = accountNumber });
    }

    public async Task AddMovementAsync(Movimento movimento)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        const string sql = """
        INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor)
        VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)
        """;
        await connection.ExecuteAsync(sql, movimento);
    }

    public async Task<decimal> GetCurrentBalanceAsync(Guid accountId)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        const string sql = """
        SELECT COALESCE(SUM(CASE WHEN tipomovimento = 'C' THEN valor ELSE -valor END), 0.0) 
        FROM movimento 
        WHERE idcontacorrente = @AccountId
        """;

        var balance = await connection.ExecuteScalarAsync<decimal>(sql, new { AccountId = accountId });

        return balance;
    }
}