using MediatR;

namespace Account.Application.Features.Accounts.Queries.Login;

public class LoginQuery : IRequest<LoginQueryResponse>
{
    public string CpfOuConta { get; set; }
    public string Senha { get; set; }
}