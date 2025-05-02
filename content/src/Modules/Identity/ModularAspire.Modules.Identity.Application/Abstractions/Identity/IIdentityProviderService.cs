using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Application.Abstractions.Identity;

public interface IIdentityProviderService
{
    Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default);
}