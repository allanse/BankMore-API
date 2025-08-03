namespace Account.Application.Contracts;

public interface IIdempotentCommand
{
    Guid IdRequisicao { get; set; }
}