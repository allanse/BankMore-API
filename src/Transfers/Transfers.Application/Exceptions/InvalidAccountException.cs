namespace Transfers.Application.Exceptions;

public class InvalidAccountException : Exception
{
    public string FailureType { get; } = "INVALID_ACCOUNT";

    public InvalidAccountException(string message) : base(message)
    {
    }
}