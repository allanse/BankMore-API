using Account.Domain;

namespace Account.Application.Contracts;

public interface ICurrentAccountRepository
{
    Task<bool> CpfExistsAsync(string cpf);
    Task<int> GetNextAccountNumberAsync();
    Task CreateAsync(CurrentAccount conta);
    Task<CurrentAccount?> GetByCpfOrAccountNumberAsync(string cpfOrAccountNumber);
    Task<CurrentAccount?> GetByIdAsync(Guid accountId);
    Task UpdateAsync(CurrentAccount account);
    Task<CurrentAccount?> GetByAccountNumberAsync(int accountNumber);
    Task AddMovementAsync(Movimento movimento);
    Task<decimal> GetCurrentBalanceAsync(Guid accountId);
}