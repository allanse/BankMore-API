using MediatR;

namespace Account.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommand : IRequest<CreateAccountResponse>
{
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Password { get; set; }
}