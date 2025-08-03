// src/Account/Domain/Movimento.cs

namespace Account.Domain;

public class Movimento
{    
    public Guid IdMovimento { get; private set; }
    
    public Guid IdContaCorrente { get; private set; }
    
    public DateTime DataMovimento { get; private set; }
    
    public char TipoMovimento { get; private set; }
    
    public decimal Valor { get; private set; }
    
    private Movimento() { }

    public static Movimento Create(Guid idContaCorrente, char tipoMovimento, decimal valor)
    {        
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