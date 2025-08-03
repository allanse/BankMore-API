namespace Account.Application.Contracts;

public interface IIdempotencyService
{    
    
    Task<string?> GetSavedResponseAsync(Guid requestId);

    Task StoreResponseAsync(Guid requestId, object request, int httpStatusCode, object response);
}