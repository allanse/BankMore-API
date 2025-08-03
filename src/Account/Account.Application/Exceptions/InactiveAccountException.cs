namespace Account.Application.Exceptions;

public class InactiveAccountException : Exception
{
    public string FailureType { get; } = "INACTIVE_ACCOUNT";
    public InactiveAccountException(string message) : base(message) { }
}