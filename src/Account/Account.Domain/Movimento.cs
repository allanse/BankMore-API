// src/Account/Domain/Movimento.cs

namespace Account.Domain;

public class Movimento
{
    // Mapeado para 'idmovimento'
    public Guid IdMovimento { get; private set; }

    // Mapeado para 'idcontacorrente'
    public Guid IdContaCorrente { get; private set; }

    // Mapeado para 'datamovimento'
    public DateTime DataMovimento { get; private set; }

    // Mapeado para 'tipomovimento' ('C' para Crédito, 'D' para Débito)
    public char TipoMovimento { get; private set; }

    // Mapeado para 'valor'
    public decimal Valor { get; private set; }

    // Construtor privado para Dapper
    private Movimento() { }

    public static Movimento Create(Guid idContaCorrente, char tipoMovimento, decimal valor)
    {
        // Validações de domínio
        if (valor <= 0)
        {
            throw new DomainException("O valor da movimentação deve ser positivo.");
        }
        if (tipoMovimento != 'C' && tipoMovimento != 'D')
        {
            throw new DomainException("Tipo de movimentação inválido.");
        }

        return new Movimento
        {
            IdMovimento = Guid.NewGuid(),
            IdContaCorrente = idContaCorrente,
            TipoMovimento = tipoMovimento,
            Valor = valor,
            DataMovimento = DateTime.UtcNow
        };
    }
}