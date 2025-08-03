using MediatR;
using Transfers.Application.Contracts;
namespace Transfers.Application.Features.Transfers.Commands.InitiateTransfer;

public class InitiateTransferCommand : IRequest<Unit>, IIdempotentCommand
{
    public Guid IdRequisicao { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}