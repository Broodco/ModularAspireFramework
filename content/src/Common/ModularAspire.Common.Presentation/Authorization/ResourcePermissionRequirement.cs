using Microsoft.AspNetCore.Authorization;

namespace ModularAspire.Common.Presentation.Authorization;

public class ResourcePermissionRequirement(string permission, Guid resourceId) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
    public Guid ResourceId { get; } = resourceId;
}