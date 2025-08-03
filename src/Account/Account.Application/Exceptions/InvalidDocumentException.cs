namespace Account.Application.Exceptions;

public class InvalidDocumentException : Exception
{    
    public string FailureType { get; } = "INVALID_DOCUMENT";

    public InvalidDocumentException(string message) : base(message)
    {
    }
}