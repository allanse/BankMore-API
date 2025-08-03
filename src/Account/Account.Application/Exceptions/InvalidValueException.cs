namespace Account.Application.Exceptions;

public class InvalidValueException : Exception
{
    public string FailureType { get; } = "INVALID_VALUE";
    public InvalidValueException(string message) : base(message) { }
}