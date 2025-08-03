namespace Account.Application.Features.Accounts.Queries.GetAccountBalance;

public class GetAccountBalanceQueryResponse
{
    public int NumeroConta { get; set; }
    public string NomeTitular { get; set; }
    public DateTime DataConsulta { get; set; }
    public decimal Saldo { get; set; }
}