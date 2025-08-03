using Account.Application.Contracts;
using Account.Application.Exceptions;
using Account.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Account.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, CreateAccountResponse>
{
    private readonly ICurrentAccountRepository _repository;
    private readonly IPasswordHasher<CurrentAccount> _passwordHasher;

    public CreateAccountCommandHandler(
        ICurrentAccountRepository repository,
        IPasswordHasher<CurrentAccount> passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<CreateAccountResponse> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        //aqui eu poderia usar um fluentValidator mas para esse portifolio vou deixar mais simples
        if (string.IsNullOrWhiteSpace(request.Cpf) || request.Cpf.Length != 11)
        {
            throw new InvalidDocumentException("O CPF é obrigatório e deve conter 11 dígitos numéricos.");
        }

        if (await _repository.CpfExistsAsync(request.Cpf))
        {
            throw new InvalidDocumentException("O CPF informado já foi cadastrado!");
        }

        var passwordHash = _passwordHasher.HashPassword(null, request.Password);

        var newAccount = CurrentAccount.Create(request.Nome, request.Cpf, passwordHash); ;

        var nextAccountNumber = await _repository.GetNextAccountNumberAsync();
        newAccount.SetAccountNumber(nextAccountNumber);

        await _repository.CreateAsync(newAccount);

        return new CreateAccountResponse
        {
            Numero = newAccount.Numero
        };
    }
}