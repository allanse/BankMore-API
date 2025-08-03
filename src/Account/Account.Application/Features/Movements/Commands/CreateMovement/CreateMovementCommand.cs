using Account.Application.Contracts;
using MediatR;

namespace Account.Application.Features.Movements.Commands.CreateMovement;

public class CreateMovementCommand : IRequest<Unit>, IIdempotentCommand
{    
    public Guid IdRequisicao { get; set; }    
    public int? NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public char TipoMovimento { get; set; }
}