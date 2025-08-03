namespace Transfers.Application.Exceptions;

public class DuplicateRequestException : Exception
{
    public DuplicateRequestException(string message) : base(message) { }
}