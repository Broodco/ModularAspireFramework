using ModularAspire.Common.Domain;

namespace ModularAspire.Modules.Identity.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(string userId) =>
        Error.NotFound("Role.NotFound", $"The user with the id {userId} was not found");

}