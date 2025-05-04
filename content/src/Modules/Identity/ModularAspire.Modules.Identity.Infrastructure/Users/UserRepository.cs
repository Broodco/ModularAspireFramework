using Microsoft.EntityFrameworkCore;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.Infrastructure.Database;

namespace ModularAspire.Modules.Identity.Infrastructure.Users;

internal sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
}