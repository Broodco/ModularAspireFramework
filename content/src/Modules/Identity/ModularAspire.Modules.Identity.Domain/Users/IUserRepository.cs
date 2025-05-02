namespace ModularAspire.Modules.Identity.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
}