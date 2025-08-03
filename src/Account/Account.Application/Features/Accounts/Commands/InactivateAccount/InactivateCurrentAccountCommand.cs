using MediatR;

namespace Account.Application.Features.Accounts.Commands.InactivateAccount;

public class InactivateCurrentAccountCommand : IRequest<Unit>
{
    public string Senha { get; set; }
}