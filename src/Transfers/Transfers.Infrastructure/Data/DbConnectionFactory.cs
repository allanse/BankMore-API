using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
namespace Transfers.Infrastructure.Data;

public interface IDbConnectionFactory { Task<IDbConnection> CreateConnectionAsync(); }

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;
    public DbConnectionFactory(IConfiguration configuration) { _configuration = configuration; }
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}