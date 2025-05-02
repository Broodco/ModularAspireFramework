namespace ModularAspire.Common.Application.Authorization;

public interface IModuleAuthorizationService
{
    Task<bool> HasPermissionAsync(string userId, string permission, Guid resourceId);
}