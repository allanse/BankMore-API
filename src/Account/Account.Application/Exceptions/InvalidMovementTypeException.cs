namespace Account.Application.Exceptions;

public class InvalidMovementTypeException : Exception
{
    public string FailureType { get; } = "INVALID_TYPE";
    public InvalidMovementTypeException(string message) : base(message) { }
}