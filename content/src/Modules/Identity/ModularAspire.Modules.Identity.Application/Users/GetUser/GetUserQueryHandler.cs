using ModularAspire.Common.Domain;
using ModularAspire.Modules.Identity.Domain.Users;

namespace ModularAspire.Modules.Identity.Application.Users.GetUser;

internal sealed class GetUserQueryHandler(IUserRepository userRepository)
{
    public async Task<Result<User?>> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);
        
        return user ?? Result.Failure<User?>(UserErrors.NotFound(query.UserId));
    }
}