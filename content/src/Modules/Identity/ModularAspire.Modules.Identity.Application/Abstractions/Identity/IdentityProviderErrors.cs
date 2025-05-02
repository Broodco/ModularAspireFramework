using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Application.Abstractions.Identity;

public static class IdentityProviderErrors
{
    public static readonly Error EmailIsNotUnique = Error.Conflict("Identity.EmailIsNotUnique", "Email is already taken.");
}