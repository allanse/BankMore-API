namespace Account.Application.Exceptions;

public class InsufficientFundsException : Exception
{
    public string FailureType { get; } = "INSUFFICIENT_FUNDS";
    public InsufficientFundsException(string message) : base(message) { }
}