using Account.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data;

namespace Account.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDbConnectionFactory));
            
            services.AddSingleton<IDbConnectionFactory>(sp =>
                new InMemoryDbConnectionFactory());
        });
    }
}

public class InMemoryDbConnectionFactory : IDbConnectionFactory, IDisposable
{
    private readonly string _connectionString = "Data Source=TestDb;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _masterConnection;

    public InMemoryDbConnectionFactory()
    {        
        _masterConnection = new SqliteConnection(_connectionString);
        _masterConnection.Open();
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {        
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public void Dispose()
    {
        _masterConnection.Dispose();
    }
}