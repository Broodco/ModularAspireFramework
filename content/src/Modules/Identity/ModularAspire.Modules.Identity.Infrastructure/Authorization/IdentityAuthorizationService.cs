using Microsoft.AspNetCore.Identity;
using ModularAspire.Common.Application.Authorization;
using ModularAspire.Modules.Identity.Domain.Users;
using ModularAspire.Modules.Identity.Infrastructure.Database;

namespace ModularAspire.Modules.Identity.Infrastructure.Authorization;

public class IdentityAuthorizationService(IdentityDbContext dbContext, UserManager<User> userManager)
    : IModuleAuthorizationService
{
    public async Task<bool> HasPermissionAsync(string userId, string permission, Guid resourceId)
    {
        if (await IsSuperAdminAsync(userId))
            return true;
        
        return false;
    }
    
    private async Task<bool> IsSuperAdminAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user != null && await userManager.IsInRoleAsync(user, "SuperAdmin");
    }
    
}