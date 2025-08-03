using Transfers.Domain;

namespace Transfers.Application.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(Transferencia account);
}