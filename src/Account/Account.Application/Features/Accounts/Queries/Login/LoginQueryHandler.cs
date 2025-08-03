using Account.Application.Contracts;
using Account.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Account.Application.Features.Accounts.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginQueryResponse>
{
    private readonly ICurrentAccountRepository _repository;
    private readonly IPasswordHasher<CurrentAccount> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginQueryHandler(
        ICurrentAccountRepository repository,
        IPasswordHasher<CurrentAccount> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginQueryResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {        
        var account = await _repository.GetByCpfOrAccountNumberAsync(request.CpfOuConta);

        if (account is null)
        {            
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
        }
        
        var verificationResult = _passwordHasher.VerifyHashedPassword(account, account.Senha, request.Senha);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
        }
        
        var token = _jwtTokenGenerator.GenerateToken(account);
        
        return new LoginQueryResponse { Token = token };
    }
}