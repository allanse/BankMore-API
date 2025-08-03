namespace Transfers.Domain;

public class Transferencia
{
    public Guid IdTransferencia { get; private set; }
    public Guid IdContaCorrenteOrigem { get; private set; }
    public Guid IdContaCorrenteDestino { get; private set; }
    public DateTime DataMovimento { get; private set; }
    public decimal Valor { get; private set; }

    private Transferencia() { }

    public static Transferencia Create(Guid origem, Guid destino, decimal valor)
    {
        return new Transferencia
        {
            IdTransferencia = Guid.NewGuid(),
            IdContaCorrenteOrigem = origem,
            IdContaCorrenteDestino = destino,
            Valor = valor,
            DataMovimento = DateTime.UtcNow
        };
    }
}