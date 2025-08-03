using Transfers.Domain;
namespace Transfers.Application.Contracts;

public interface ITransferRepository
{
    Task AddAsync(Transferencia transferencia);
}