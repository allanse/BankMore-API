using Account.Domain;

namespace Account.Application.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(CurrentAccount account);
}