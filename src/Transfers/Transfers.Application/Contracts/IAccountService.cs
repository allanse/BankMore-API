namespace Transfers.Application.Contracts;

public interface IAccountService
{
    Task PerformMovementAsync(MovementRequest request);
    Task<AccountDetailsResponse?> GetAccountDetailsByNumberAsync(int accountNumber);
    Task<AccountDetailsResponse?> GetAccountDetailsByIdAsync(Guid accountId);
}

public record MovementRequest(int? NumeroConta, decimal Valor, char TipoMovimento);
public record AccountDetailsResponse(Guid IdContaCorrente, bool Ativo);