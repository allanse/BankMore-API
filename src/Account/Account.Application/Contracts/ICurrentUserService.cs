namespace Account.Application.Contracts;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}