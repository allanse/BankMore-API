using Account.Application.Contracts;
using Account.Application.Exceptions;
using Account.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Account.Application.Features.Accounts.Commands.InactivateAccount;

public class InactivateCurrentAccountCommandHandler : IRequestHandler<InactivateCurrentAccountCommand, Unit>
{
    private readonly ICurrentAccountRepository _repository;
    private readonly IPasswordHasher<CurrentAccount> _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public InactivateCurrentAccountCommandHandler(
        ICurrentAccountRepository repository,
        IPasswordHasher<CurrentAccount> passwordHasher,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(InactivateCurrentAccountCommand request, CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentUserId();

        var account = await _repository.GetByIdAsync(accountId);
        if (account is null)
        {
            throw new InvalidAccountException("A conta informada não existe.");
        }
 
        var verificationResult = _passwordHasher.VerifyHashedPassword(account, account.Senha, request.Senha);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Senha inválida.");
        }

        account.Inactivate();

        await _repository.UpdateAsync(account);

        return Unit.Value;
    }
}