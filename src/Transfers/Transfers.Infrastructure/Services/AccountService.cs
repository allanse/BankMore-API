using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Transfers.Application.Contracts;

namespace Transfers.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _accountApiUrl;

    public AccountService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _accountApiUrl = configuration["ServiceSettings:AccountApiUrl"]!;
    }

    public async Task PerformMovementAsync(MovementRequest request)
    {
        var httpClient = _httpClientFactory.CreateClient();
        
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
        }
        
        var idempotencyKey = Guid.NewGuid();
        httpClient.DefaultRequestHeaders.Add("X-Idempotency-Key", idempotencyKey.ToString());

        var content = JsonContent.Create(request);
        
        var response = await httpClient.PostAsync($"{_accountApiUrl}/api/v1/movements", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();            
            var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
            var errorMessage = errorResponse.TryGetProperty("mensagem", out var msg) ? msg.GetString() : "Erro desconhecido.";
            throw new HttpRequestException(errorMessage, null, response.StatusCode);
        }
    }

    public async Task<AccountDetailsResponse?> GetAccountDetailsByNumberAsync(int accountNumber)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
        }

        var response = await httpClient.GetAsync($"{_accountApiUrl}/api/v1/accounts/{accountNumber}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AccountDetailsResponse>();
    }

    public async Task<AccountDetailsResponse?> GetAccountDetailsByIdAsync(Guid accountId)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
        }
        var response = await httpClient.GetAsync($"{_accountApiUrl}/api/v1/accounts/by-id/{accountId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AccountDetailsResponse>();
    }
}