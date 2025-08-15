namespace BankMore.Common.Messaging.Events;

public class TransferenciaIniciadaEvent
{
    public Guid IdRequisicao { get; set; }
    public Guid IdContaOrigem { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}