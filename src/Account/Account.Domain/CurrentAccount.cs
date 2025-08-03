namespace Account.Domain;

public class CurrentAccount
{
    public Guid IdContaCorrente { get; private set; }
    
    public int Numero { get; private set; }

    public string Nome { get; private set; }

    public string Senha { get; private set; }

    public bool Ativo { get; private set; }

    public string Cpf { get; private set; }   

    private CurrentAccount() { }

    public static CurrentAccount Create(string nome, string cpf, string senhaHash)
    {
        // Aqui podemos adicionar validações de domínio, se necessário.
        // Por exemplo, ownerName não pode ser nulo, etc.

        return new CurrentAccount
        {
            IdContaCorrente = Guid.NewGuid(),
            Nome = nome,
            Cpf = cpf,
            Senha = senhaHash,
            Ativo = true
        };
    }

    public void SetAccountNumber(int numero)
    {
        if (Numero == 0)
        {
            Numero = numero;
        }
    }

    public void Inactivate()
    {
        if (!Ativo)
        {
            throw new DomainException("A conta já está inativa.");
        }
        Ativo = false;
    }
}