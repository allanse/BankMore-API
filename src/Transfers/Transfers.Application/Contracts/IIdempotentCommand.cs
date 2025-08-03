namespace Transfers.Application.Contracts;

public interface IIdempotentCommand
{
    Guid IdRequisicao { get; set; }
}