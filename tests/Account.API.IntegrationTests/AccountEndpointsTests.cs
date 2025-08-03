using System.Net;
using System.Net.Http.Json;
using Account.Application.Features.Accounts.Commands.CreateAccount;
using Account.Infrastructure.Data;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Account.API.IntegrationTests;

public class AccountEndpointsTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public AccountEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();

        _dbConnectionFactory = _scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        
        var initializer = _scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        initializer.InitializeAsync().Wait();
    }
    
    public void Dispose()
    {        
        using var connection = _dbConnectionFactory.CreateConnectionAsync().Result;
        connection.Execute("DELETE FROM contacorrente");
        _scope.Dispose();
    }

    [Fact]
    public async Task CreateAccount_WithValidData_ShouldReturnCreated()
    {        
        // Arrange
        var command = new CreateAccountCommand
        {
            Nome = "Fulano Detal",
            Cpf = "11122233344",
            Password = "Fulano!123"
        };
        var content = JsonContent.Create(command);

        // Act
        var response = await _client.PostAsync("/api/v1/accounts", content);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdAccount = await response.Content.ReadFromJsonAsync<CreateAccountResponse>();
        createdAccount.Should().NotBeNull();
        createdAccount!.Numero.Should().BeGreaterThan(1000);
    }

    [Fact]
    public async Task CreateAccount_WithDuplicateCpf_ShouldReturnConflict()
    {        
        // Arrange
        var command = new CreateAccountCommand
        {
            Nome = "Ever Green",
            Cpf = "55566677788",
            Password = "Green!456"
        };
        var content = JsonContent.Create(command);

        // Act
        var firstResponse = await _client.PostAsync("/api/v1/accounts", content);
        var secondResponse = await _client.PostAsync("/api/v1/accounts", content);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}