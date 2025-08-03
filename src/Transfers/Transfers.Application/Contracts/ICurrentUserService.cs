namespace Transfers.Application.Contracts;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}